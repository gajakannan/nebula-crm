#!/usr/bin/env python3
from __future__ import annotations

import argparse
import json
import sys
from typing import Any

from kg_common import (
    edge_ref_id,
    edge_ref_ids,
    edge_ref_provenance,
    feature_or_story_by_id,
    load_bundle,
    match_bindings_for_path,
    normalize_target_id,
    planning_scope_for_path,
    related_mapping_entries,
    repo_relative,
    resolve_node,
    resolve_refs,
)


def build_scope_payload(target: dict[str, Any], bundle: dict[str, Any]) -> dict[str, Any]:
    payload: dict[str, Any] = {
        "target": target,
        "source_precedence": bundle["ontology"]["authority"]["precedence"],
    }

    if target.get("_kind") == "story" and target.get("feature"):
        feature = resolve_node(target["feature"], bundle)
        if feature is not None:
            payload["feature"] = feature

    provenance_annotations: dict[str, list[dict[str, Any]]] = {}

    for field in (
        "affects",
        "workflow_states",
        "restricted_to_role",
        "enforced_by_policy",
        "governed_by",
        "uses_schema",
        "uses_api_contract",
        "depends_on",
        "validated_by",
        "supersedes",
    ):
        refs = target.get(field, [])
        if refs:
            ref_ids = edge_ref_ids(refs)
            payload[field] = resolve_refs(ref_ids, bundle)
            # Collect provenance for edges that have it
            for ref in refs:
                prov = edge_ref_provenance(ref)
                if prov is not None:
                    provenance_annotations.setdefault(field, []).append(
                        {"id": edge_ref_id(ref), **prov}
                    )

    if provenance_annotations:
        payload["provenance"] = provenance_annotations

    return payload


def lookup_by_target(target: str, bundle: dict[str, Any]) -> dict[str, Any]:
    normalized = normalize_target_id(target)

    scope = feature_or_story_by_id(normalized, bundle["mappings"])
    if scope is not None:
        scope["_kind"] = "feature" if normalized.startswith("feature:") else "story"
        return build_scope_payload(scope, bundle)

    node = resolve_node(normalized, bundle)
    if node is None:
        raise SystemExit(f"Unknown target: {target}")

    related_features, related_stories = related_mapping_entries([normalized], bundle["mappings"])
    return {
        "target": node,
        "related_features": related_features,
        "related_stories": related_stories,
        "source_precedence": bundle["ontology"]["authority"]["precedence"],
    }


def lookup_by_file(path: str, bundle: dict[str, Any]) -> dict[str, Any]:
    binding_matches = match_bindings_for_path(path, bundle)
    node_ids = [match["id"] for match in binding_matches]
    planning_scope = planning_scope_for_path(path, bundle["mappings"])
    related_features, related_stories = related_mapping_entries(node_ids, bundle["mappings"])

    return {
        "query": {"file": repo_relative(path)},
        "matched_node_ids": node_ids,
        "matched_nodes": [resolve_node(node_id, bundle) for node_id in node_ids],
        "planning_scope": planning_scope,
        "related_features": related_features,
        "related_stories": related_stories,
        "matched_bindings": [
            {
                "id": match["id"],
                "matched_patterns": match["matched_patterns"],
                "paths": match.get("paths", {}),
            }
            for match in binding_matches
        ],
        "source_precedence": bundle["ontology"]["authority"]["precedence"],
    }


def main() -> int:
    parser = argparse.ArgumentParser(
        description="Resolve ontology-backed planning scope and code bindings."
    )
    parser.add_argument("target", nargs="?", help="Feature/story ID such as F0007 or F0007-S0003")
    parser.add_argument(
        "--file",
        dest="file_path",
        help="Reverse lookup for a repo file path such as engine/src/.../Submission.cs",
    )
    args = parser.parse_args()

    if not args.target and not args.file_path:
        parser.error("Provide a target ID or --file.")

    if args.target and args.file_path:
        parser.error("Use either a target ID or --file, not both.")

    bundle = load_bundle()
    payload = (
        lookup_by_file(args.file_path, bundle)
        if args.file_path
        else lookup_by_target(args.target, bundle)
    )
    json.dump(payload, sys.stdout, indent=2)
    sys.stdout.write("\n")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())

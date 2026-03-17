import { useEffect, useRef, useState, type KeyboardEvent } from 'react';
import { Popover } from '@/components/ui/Popover';
import { opportunityHex, opportunityText } from '../lib/opportunity-colors';
import { OpportunityPopoverContent } from './OpportunityPopover';
import { TerminalOutcomesRail } from './TerminalOutcomesRail';
import { ChapterOverlayManager } from './ChapterOverlayManager';
import { StageNodeStoryPanel } from './StageNodeStoryPanel';
import type {
  OpportunityAgingDto,
  OpportunityFlowDto,
  OpportunityFlowNodeDto,
  OpportunityOutcomeDto,
} from '../types';
import type { StoryChapter } from './storyTypes';
import { cn } from '@/lib/utils';

export interface StageAnchor {
  status: string;
  label: string;
  x: number;
  y: number;
  avgDwellDays?: number | null;
  emphasis?: OpportunityFlowNodeDto['emphasis'];
}

export interface OutcomeAnchor {
  key: string;
  label: string;
  branchStyle: OpportunityOutcomeDto['branchStyle'];
  count: number;
  percentOfTotal: number;
  x: number;
  y: number;
}

interface ConnectedFlowProps {
  flow: OpportunityFlowDto;
  outcomes: OpportunityOutcomeDto[];
  chapter: StoryChapter;
  periodDays: number;
  outcomesLoading: boolean;
  outcomesError: boolean;
  onRetryOutcomes: () => void;
  aging?: OpportunityAgingDto;
}

const CANVAS_MAX_WIDTH = 1160;
const STAGE_START_Y = 108;
const STAGE_GAP = 146;
const NODE_OFFSET_X = 98;
const NODE_WIDTH = 132;
const SIDE_OFFSET_X = 304;
const OUTCOME_GAP_X = 170;
const OUTCOME_OFFSET_Y = 110;

function clamp(value: number, min: number, max: number): number {
  return Math.min(max, Math.max(min, value));
}

function buildStageLinkPath(source: StageAnchor, target: StageAnchor, spineX: number, nodeWidth: number): string {
  const startX = source.x < spineX ? source.x + nodeWidth / 2 - 8 : source.x - nodeWidth / 2 + 8;
  const endX = target.x < spineX ? target.x + nodeWidth / 2 - 8 : target.x - nodeWidth / 2 + 8;

  return `M ${startX} ${source.y} C ${spineX} ${source.y}, ${spineX} ${target.y}, ${endX} ${target.y}`;
}

function buildOutcomePath(source: StageAnchor, target: OutcomeAnchor, spineX: number, nodeWidth: number): string {
  const startX = source.x < spineX ? source.x + nodeWidth / 2 - 8 : source.x - nodeWidth / 2 + 8;
  const midY = source.y + (target.y - source.y) * 0.52;

  return `M ${startX} ${source.y} C ${spineX} ${midY}, ${target.x} ${midY}, ${target.x} ${target.y}`;
}

function branchStroke(
  branchStyle: OpportunityOutcomeDto['branchStyle'],
  muted = false,
) {
  if (muted) {
    if (branchStyle === 'gray_dotted') {
      return {
        stroke: 'color-mix(in srgb, var(--text-muted) 58%, transparent)',
        strokeDasharray: '1 7' as string | undefined,
      };
    }

    if (branchStyle === 'red_dashed') {
      return {
        stroke: 'color-mix(in srgb, var(--text-muted) 58%, transparent)',
        strokeDasharray: '8 6' as string | undefined,
      };
    }

    return {
      stroke: 'color-mix(in srgb, var(--text-muted) 58%, transparent)',
      strokeDasharray: undefined as string | undefined,
    };
  }

  if (branchStyle === 'solid') {
    return { stroke: 'var(--color-status-success)', strokeDasharray: undefined as string | undefined };
  }

  if (branchStyle === 'gray_dotted') {
    return { stroke: 'var(--text-muted)', strokeDasharray: '1 7' };
  }

  return { stroke: 'var(--color-status-error)', strokeDasharray: '8 6' };
}

export function ConnectedFlow({
  flow,
  outcomes,
  chapter,
  periodDays,
  outcomesLoading,
  outcomesError,
  onRetryOutcomes,
  aging,
}: ConnectedFlowProps) {
  const containerRef = useRef<HTMLDivElement>(null);
  const stageButtonRefs = useRef<Array<HTMLButtonElement | null>>([]);
  const [availableWidth, setAvailableWidth] = useState(CANVAS_MAX_WIDTH);

  useEffect(() => {
    if (!containerRef.current || typeof ResizeObserver === 'undefined') {
      return;
    }

    const observer = new ResizeObserver((entries) => {
      const entry = entries[0];
      if (entry) {
        setAvailableWidth(Math.round(entry.contentRect.width));
      }
    });

    observer.observe(containerRef.current);
    return () => observer.disconnect();
  }, []);

  const stageNodes = flow.nodes
    .filter((node) => !node.isTerminal)
    .sort((left, right) => left.displayOrder - right.displayOrder);

  if (stageNodes.length === 0) {
    return (
      <div className="py-8 text-center text-sm text-text-muted">
        No opportunity flow data for the selected period.
      </div>
    );
  }

  const canvasWidth = clamp(availableWidth || CANVAS_MAX_WIDTH, 340, CANVAS_MAX_WIDTH);
  const spineX = canvasWidth / 2;
  const phoneLayout = canvasWidth < 560;
  const compactLayout = canvasWidth < 900;
  const nodeOffsetX = phoneLayout ? 0 : compactLayout ? Math.max(62, canvasWidth * 0.13) : NODE_OFFSET_X;
  const nodeWidth = compactLayout ? 118 : NODE_WIDTH;
  const sideOffsetX = phoneLayout ? 0 : compactLayout ? Math.max(168, canvasWidth * 0.24) : SIDE_OFFSET_X;
  const stageGap = phoneLayout ? 214 : compactLayout ? 164 : STAGE_GAP;
  const stageStartY = phoneLayout ? 132 : STAGE_START_Y;
  const outcomeGapX = phoneLayout
    ? Math.max(72, Math.min(124, canvasWidth / (Math.max(outcomes.length, 1) + 0.2)))
    : compactLayout
      ? 132
      : OUTCOME_GAP_X;
  const outcomeOffsetY = phoneLayout ? 132 : OUTCOME_OFFSET_Y;
  const allStageCountsZero = stageNodes.every((node) => node.currentCount === 0);

  const stageAnchors: StageAnchor[] = stageNodes.map((node, index) => ({
    status: node.status,
    label: node.label,
    x: spineX + (index % 2 === 0 ? -nodeOffsetX : nodeOffsetX),
    y: stageStartY + index * stageGap,
    avgDwellDays: node.avgDwellDays,
    emphasis: node.emphasis,
  }));

  const stageLayouts = stageAnchors.map((anchor, index) => {
    const node = stageNodes[index];
    const miniOnLeft = index % 2 === 0;

    return {
      anchor,
      node,
      miniX: phoneLayout ? spineX : spineX + (miniOnLeft ? -sideOffsetX : sideOffsetX),
      calloutX: phoneLayout ? spineX : spineX + (miniOnLeft ? sideOffsetX : -sideOffsetX),
      miniY: phoneLayout ? anchor.y - 68 : anchor.y,
      calloutY: phoneLayout ? anchor.y + 76 : anchor.y,
      stackedStop: phoneLayout,
    };
  });

  const lastStage = stageAnchors[stageAnchors.length - 1];
  const timelineBottomY = lastStage.y + 86;
  const outcomeY = timelineBottomY + outcomeOffsetY;

  const outcomeStartX = spineX - ((outcomes.length - 1) * outcomeGapX) / 2;
  const outcomeAnchors: OutcomeAnchor[] = outcomes.map((outcome, index) => ({
    key: outcome.key,
    label: outcome.label,
    branchStyle: outcome.branchStyle,
    count: outcome.count,
    percentOfTotal: outcome.percentOfTotal,
    x: outcomeStartX + index * outcomeGapX,
    y: outcomeY,
  }));

  const canvasHeight = outcomeAnchors.length > 0
    ? outcomeY + 140
    : timelineBottomY + 140;
  const allOutcomesZero = outcomeAnchors.length > 0 && outcomeAnchors.every((outcome) => outcome.count === 0);

  const stageByStatus = new Map(stageAnchors.map((anchor) => [anchor.status, anchor]));
  const stageNodeByStatus = new Map(stageNodes.map((node) => [node.status, node]));

  const stageLinks = flow.links
    .map((link) => ({
      link,
      source: stageByStatus.get(link.sourceStatus),
      target: stageByStatus.get(link.targetStatus),
      sourceNode: stageNodeByStatus.get(link.sourceStatus),
    }))
    .filter(
      (entry): entry is {
        link: OpportunityFlowDto['links'][number];
        source: StageAnchor;
        target: StageAnchor;
        sourceNode: OpportunityFlowNodeDto;
      } => Boolean(entry.source && entry.target && entry.sourceNode),
    );

  const maxLinkCount = Math.max(1, ...stageLinks.map((entry) => entry.link.count));

  function moveStageFocus(targetIndex: number) {
    if (stageNodes.length === 0) {
      return;
    }

    const bounded = ((targetIndex % stageNodes.length) + stageNodes.length) % stageNodes.length;
    stageButtonRefs.current[bounded]?.focus();
  }

  function onStageKeyDown(event: KeyboardEvent<HTMLButtonElement>, stageIndex: number) {
    if (event.key === 'ArrowDown' || event.key === 'ArrowRight') {
      event.preventDefault();
      moveStageFocus(stageIndex + 1);
    }

    if (event.key === 'ArrowUp' || event.key === 'ArrowLeft') {
      event.preventDefault();
      moveStageFocus(stageIndex - 1);
    }

    if (event.key === 'Home') {
      event.preventDefault();
      moveStageFocus(0);
    }

    if (event.key === 'End') {
      event.preventDefault();
      moveStageFocus(stageNodes.length - 1);
    }
  }

  return (
    <div ref={containerRef} className="canvas-chapter-overlay relative max-h-[780px] overflow-y-auto overflow-x-hidden">
      <div className="relative mx-auto w-full" style={{ width: canvasWidth, height: canvasHeight }}>
        <svg
          aria-hidden="true"
          className="absolute inset-0 h-full w-full"
          viewBox={`0 0 ${canvasWidth} ${canvasHeight}`}
        >
          <line
            x1={spineX}
            y1={stageStartY - 72}
            x2={spineX}
            y2={timelineBottomY}
            stroke="color-mix(in srgb, var(--text-muted) 55%, transparent)"
            strokeWidth={3}
            strokeLinecap="round"
          />

          {stageAnchors.map((anchor) => (
            <g key={`spine-anchor-${anchor.status}`}>
              <line
                x1={spineX}
                y1={anchor.y}
                x2={anchor.x}
                y2={anchor.y}
                stroke="color-mix(in srgb, var(--text-muted) 45%, transparent)"
                strokeWidth={2}
                strokeLinecap="round"
              />
              <circle
                cx={spineX}
                cy={anchor.y}
                r={3.5}
                fill="color-mix(in srgb, var(--accent-secondary) 60%, transparent)"
              />
            </g>
          ))}

          {stageLinks.map(({ link, source, target, sourceNode }) => (
            <path
              key={`${link.sourceStatus}-${link.targetStatus}`}
              d={buildStageLinkPath(source, target, spineX, nodeWidth)}
              fill="none"
              stroke={opportunityHex(sourceNode.colorGroup)}
              strokeOpacity={chapter === 'outcomes' ? 0.22 : 0.58}
              strokeWidth={2 + (link.count / maxLinkCount) * 8}
              strokeLinecap="round"
            />
          ))}

          {outcomeAnchors.map((outcomeAnchor) => {
            const stroke = branchStroke(outcomeAnchor.branchStyle, allOutcomesZero);
            return (
              <path
                key={`branch-${outcomeAnchor.key}`}
                d={buildOutcomePath(lastStage, outcomeAnchor, spineX, nodeWidth)}
                fill="none"
                stroke={stroke.stroke}
                strokeDasharray={stroke.strokeDasharray}
                strokeOpacity={chapter === 'outcomes' ? (allOutcomesZero ? 0.56 : 1) : 0.74}
                strokeWidth={chapter === 'outcomes' ? 3 : 2.5}
                strokeLinecap="round"
                style={chapter === 'outcomes' && !allOutcomesZero
                  ? { filter: `drop-shadow(0 0 8px ${stroke.stroke})` }
                  : undefined}
              />
            );
          })}
        </svg>

        {stageLayouts.map(({ anchor, node, miniX, calloutX, miniY, calloutY, stackedStop }, index) => {
          const faded = node.currentCount === 0;
          const emphasisClass = chapter === 'friction'
            ? `flow-emphasis-${node.emphasis ?? 'normal'}`
            : 'flow-emphasis-normal';
          const stageTrigger = (
            <button
              ref={(element) => {
                stageButtonRefs.current[index] = element;
              }}
              type="button"
              onKeyDown={(event) => onStageKeyDown(event, index)}
              className={cn(
                'min-h-[70px] rounded-xl bg-surface-main/70 px-3 py-2 text-left shadow-sm transition-colors hover:bg-surface-main/80 focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-nebula-violet/50',
                emphasisClass,
              )}
              style={{ width: nodeWidth }}
              aria-label={`${anchor.label} stage, ${node.currentCount} opportunities`}
            >
              <p className="truncate text-xs font-semibold uppercase tracking-wide text-text-muted">
                {anchor.label}
              </p>
              <p className={cn('mt-1 text-xl font-semibold', opportunityText(node.colorGroup))}>
                {node.currentCount}
              </p>
            </button>
          );

          return (
            <div key={anchor.status}>
              {!allStageCountsZero && (
                <StageNodeStoryPanel
                  node={node}
                  entityType={flow.entityType}
                  periodDays={periodDays}
                  chapter={chapter}
                  outcomes={outcomes}
                  agingStatus={aging?.statuses.find((status) => status.status === node.status)}
                  miniX={miniX}
                  calloutX={calloutX}
                  y={anchor.y}
                  miniY={miniY}
                  calloutY={calloutY}
                  compact={compactLayout}
                  stacked={stackedStop}
                />
              )}

              <div
                className={cn(
                  'absolute -translate-x-1/2 -translate-y-1/2',
                  faded && 'opacity-60',
                  chapter === 'outcomes' && 'opacity-70',
                )}
                style={{ left: anchor.x, top: anchor.y }}
              >
                {allStageCountsZero ? (
                  stageTrigger
                ) : (
                  <Popover
                    contentAriaLabel={`${anchor.label} stage details, ${node.currentCount} opportunities in stage`}
                    trigger={stageTrigger}
                  >
                    <OpportunityPopoverContent entityType={flow.entityType} status={anchor.status} />
                  </Popover>
                )}
              </div>
            </div>
          );
        })}

        {outcomeAnchors.length === 0 ? (
          <p
            className="absolute -translate-x-1/2 text-xs font-medium text-text-muted"
            style={{ left: spineX, top: timelineBottomY + 72 }}
          >
            No exits in period
          </p>
        ) : (
          <TerminalOutcomesRail
            anchors={outcomeAnchors}
            periodDays={periodDays}
            chapter={chapter}
            allOutcomesZero={allOutcomesZero}
          />
        )}

        {allOutcomesZero && (
          <p
            className="absolute -translate-x-1/2 text-xs font-medium text-text-muted"
            style={{ left: spineX, top: outcomeY + 64 }}
          >
            No outcomes in period
          </p>
        )}

        {allStageCountsZero && (
          <p
            className="absolute -translate-x-1/2 text-xs font-medium text-text-muted"
            style={{ left: spineX, top: timelineBottomY + 30 }}
          >
            No activity in period
          </p>
        )}

        <ChapterOverlayManager
          chapter={chapter}
          stageAnchors={stageAnchors}
          outcomeAnchors={outcomeAnchors}
          outcomesLoading={outcomesLoading}
          outcomesError={outcomesError}
          onRetryOutcomes={onRetryOutcomes}
        />
      </div>
    </div>
  );
}

import { useRef, useState, useEffect, useCallback } from 'react';
import type { PipelineEntityType, PipelineStatusCountDto } from '@/types';
import { pipelineHex, pipelineHexLight } from '@/lib/pipeline-colors';
import { Popover } from '@/components/ui/Popover';
import { PipelinePopoverContent } from './PipelinePopover';
import { SankeyBands, type NodeLayout } from './SankeyBands';

interface PipelineChartProps {
  label: string;
  entityType: PipelineEntityType;
  statuses: PipelineStatusCountDto[];
}

function formatStatus(status: string): string {
  return status.replace(/([A-Z])/g, ' $1').trim();
}

const NODE_WIDTH = 80;
const NODE_GAP = 12;
const MAX_NODE_HEIGHT = 100;
const MIN_NODE_HEIGHT = 36;
const CHART_PADDING_Y = 16;

export function PipelineChart({ label, entityType, statuses }: PipelineChartProps) {
  const containerRef = useRef<HTMLDivElement>(null);
  const [containerWidth, setContainerWidth] = useState(0);
  const total = statuses.reduce((sum, s) => sum + s.count, 0);
  const maxCount = Math.max(...statuses.map((s) => s.count), 1);

  const updateWidth = useCallback(() => {
    if (containerRef.current) {
      setContainerWidth(containerRef.current.offsetWidth);
    }
  }, []);

  useEffect(() => {
    updateWidth();
    const observer = new ResizeObserver(updateWidth);
    if (containerRef.current) observer.observe(containerRef.current);
    return () => observer.disconnect();
  }, [updateWidth]);

  if (total === 0) {
    return (
      <div>
        <div className="mb-2 flex items-center justify-between">
          <h3 className="text-xs font-medium uppercase tracking-wider" style={{ color: 'var(--text-muted)' }}>{label}</h3>
        </div>
        <p className="text-xs" style={{ color: 'var(--text-muted)' }}>No items in pipeline</p>
      </div>
    );
  }

  // Compute node positions
  const nodeCount = statuses.length;
  const totalNodeWidth = nodeCount * NODE_WIDTH;
  const totalGapWidth = (nodeCount - 1) * NODE_GAP;
  const availableWidth = containerWidth - totalNodeWidth - totalGapWidth;
  const bandGap = nodeCount > 1 ? availableWidth / (nodeCount - 1) : 0;
  const stepX = NODE_WIDTH + NODE_GAP + Math.max(0, bandGap);

  const containerHeight = MAX_NODE_HEIGHT + CHART_PADDING_Y * 2;

  const nodeLayouts: NodeLayout[] = statuses.map((s, i) => {
    const height = Math.max(MIN_NODE_HEIGHT, (s.count / maxCount) * MAX_NODE_HEIGHT);
    return {
      x: i * stepX,
      y: CHART_PADDING_Y + (MAX_NODE_HEIGHT - height) / 2,
      width: NODE_WIDTH,
      height,
      colorGroup: s.colorGroup,
    };
  });

  return (
    <div>
      <div className="mb-3 flex items-center justify-between">
        <h3 className="text-xs font-medium uppercase tracking-wider" style={{ color: 'var(--text-muted)' }}>{label}</h3>
        <span className="text-xs" style={{ color: 'var(--text-muted)' }}>{total} total</span>
      </div>

      {/* Sankey chart */}
      <div
        ref={containerRef}
        className="relative"
        style={{ height: containerHeight }}
      >
        {containerWidth > 0 && (
          <>
            <SankeyBands
              nodes={nodeLayouts}
              containerWidth={containerWidth}
              containerHeight={containerHeight}
            />

            {/* Nodes */}
            {statuses.map((s, i) => {
              const layout = nodeLayouts[i];
              const hex = pipelineHex(s.colorGroup);
              const hexLight = pipelineHexLight(s.colorGroup);

              const node = (
                <div
                  className="absolute flex flex-col items-center justify-center rounded-lg text-white cursor-pointer transition-transform hover:scale-105 focus-visible:outline-2 focus-visible:outline-offset-2 focus-visible:outline-nebula-violet"
                  tabIndex={0}
                  role="button"
                  aria-label={`${formatStatus(s.status)}: ${s.count} items. Click for details`}
                  onKeyDown={(e) => {
                    if (e.key === 'Enter' || e.key === ' ') {
                      e.preventDefault();
                      (e.target as HTMLElement).click();
                    }
                  }}
                  style={{
                    left: layout.x,
                    top: layout.y,
                    width: layout.width,
                    height: layout.height,
                    background: `linear-gradient(135deg, ${hex}, ${hexLight})`,
                    boxShadow: `0 0 16px ${hex}40, inset 0 1px 0 rgba(255,255,255,0.15)`,
                  }}
                >
                  <span className="text-[10px] font-medium opacity-80 leading-tight">
                    {formatStatus(s.status)}
                  </span>
                  <span className="text-lg font-bold leading-tight">{s.count}</span>
                </div>
              );

              return (
                <Popover key={s.status} trigger={node}>
                  <PipelinePopoverContent entityType={entityType} status={s.status} />
                </Popover>
              );
            })}
          </>
        )}
      </div>

      {/* Legend */}
      <div className="mt-3 flex flex-wrap gap-x-4 gap-y-1">
        {statuses.map((s) => (
          <div key={s.status} className="flex items-center gap-1.5">
            <span
              className="h-2 w-2 rounded-full"
              style={{ background: pipelineHex(s.colorGroup) }}
            />
            <span className="text-xs" style={{ color: 'var(--text-muted)' }}>
              {formatStatus(s.status)}
            </span>
            <span className="text-xs font-medium" style={{ color: 'var(--text-secondary)' }}>{s.count}</span>
          </div>
        ))}
      </div>
    </div>
  );
}

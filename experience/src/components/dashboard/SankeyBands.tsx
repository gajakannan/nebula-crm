import type { PipelineColorGroup } from '@/types';
import { pipelineHex } from '@/lib/pipeline-colors';

export interface NodeLayout {
  x: number;
  y: number;
  width: number;
  height: number;
  colorGroup: PipelineColorGroup;
}

interface SankeyBandsProps {
  nodes: NodeLayout[];
  containerWidth: number;
  containerHeight: number;
}

export function SankeyBands({ nodes, containerWidth, containerHeight }: SankeyBandsProps) {
  if (nodes.length < 2) return null;

  const bands: React.ReactNode[] = [];

  for (let i = 0; i < nodes.length - 1; i++) {
    const source = nodes[i];
    const dest = nodes[i + 1];

    // Band width is proportional to the smaller of the two node heights
    const bandHeight = Math.min(source.height, dest.height) * 0.6;

    // Source connection points (right edge, vertically centered)
    const sx = source.x + source.width;
    const syCentre = source.y + source.height / 2;
    const syTop = syCentre - bandHeight / 2;
    const syBottom = syCentre + bandHeight / 2;

    // Target connection points (left edge, vertically centered)
    const tx = dest.x;
    const tyCentre = dest.y + dest.height / 2;
    const tyTop = tyCentre - bandHeight / 2;
    const tyBottom = tyCentre + bandHeight / 2;

    // Control points at 50% horizontal distance
    const cpx1 = sx + (tx - sx) * 0.5;
    const cpx2 = sx + (tx - sx) * 0.5;

    const gradientId = `band-gradient-${i}`;
    const hex = pipelineHex(dest.colorGroup);

    bands.push(
      <defs key={`defs-${i}`}>
        <linearGradient id={gradientId} x1="0" y1="0" x2="1" y2="0">
          <stop offset="0%" stopColor={pipelineHex(source.colorGroup)} stopOpacity={0.15} />
          <stop offset="100%" stopColor={hex} stopOpacity={0.25} />
        </linearGradient>
      </defs>,
      <path
        key={`band-${i}`}
        d={`
          M ${sx},${syTop}
          C ${cpx1},${syTop} ${cpx2},${tyTop} ${tx},${tyTop}
          L ${tx},${tyBottom}
          C ${cpx2},${tyBottom} ${cpx1},${syBottom} ${sx},${syBottom}
          Z
        `}
        fill={`url(#${gradientId})`}
        aria-hidden="true"
      />,
    );
  }

  return (
    <svg
      className="absolute inset-0 pointer-events-none"
      width={containerWidth}
      height={containerHeight}
      viewBox={`0 0 ${containerWidth} ${containerHeight}`}
      aria-hidden="true"
    >
      {bands}
    </svg>
  );
}

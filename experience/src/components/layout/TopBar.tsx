import { Menu } from 'lucide-react';
import { useSidebar } from '@/hooks/useSidebar';

export function TopBar() {
  const { openMobile } = useSidebar();

  return (
    <header className="sticky top-0 z-30 flex h-14 items-center gap-3 border-b px-4 backdrop-blur-xl lg:hidden"
      style={{
        borderColor: 'var(--sidebar-border)',
        background: 'var(--sidebar-bg)',
      }}
    >
      <button
        onClick={openMobile}
        aria-label="Open navigation menu"
        className="flex h-9 w-9 items-center justify-center rounded-md transition-colors"
        style={{ color: 'var(--text-secondary)' }}
      >
        <Menu size={20} />
      </button>
      <span className="text-lg font-bold tracking-tight text-nebula-violet drop-shadow-[0_0_8px_rgba(139,92,246,0.3)]">
        Nebula
      </span>
      <span className="text-sm" style={{ color: 'var(--text-muted)' }}>CRM</span>
    </header>
  );
}

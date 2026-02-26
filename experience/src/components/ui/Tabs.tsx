import { cn } from '@/lib/utils';

interface TabsProps {
  tabs: string[];
  activeTab: string;
  onTabChange: (tab: string) => void;
  children: React.ReactNode;
}

export function Tabs({ tabs, activeTab, onTabChange, children }: TabsProps) {
  return (
    <div>
      <div className="flex gap-1 overflow-x-auto border-b border-surface-border">
        {tabs.map((tab) => (
          <button
            key={tab}
            onClick={() => onTabChange(tab)}
            className={cn(
              'whitespace-nowrap px-4 py-2.5 text-sm font-medium transition-colors',
              tab === activeTab
                ? 'border-b-2 border-nebula-violet text-text-primary'
                : 'text-text-muted hover:text-text-secondary',
            )}
          >
            {tab}
          </button>
        ))}
      </div>
      <div className="pt-4">{children}</div>
    </div>
  );
}

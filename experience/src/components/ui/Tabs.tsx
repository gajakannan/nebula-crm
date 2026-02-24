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
      <div className="flex gap-1 overflow-x-auto border-b border-zinc-800">
        {tabs.map((tab) => (
          <button
            key={tab}
            onClick={() => onTabChange(tab)}
            className={cn(
              'whitespace-nowrap px-4 py-2.5 text-sm font-medium transition-colors',
              tab === activeTab
                ? 'border-b-2 border-nebula-violet text-zinc-200'
                : 'text-zinc-500 hover:text-zinc-300',
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

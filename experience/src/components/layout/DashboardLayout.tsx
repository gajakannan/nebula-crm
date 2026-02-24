import { SidebarContext, useSidebarProvider } from '@/hooks/useSidebar';
import { useSidebar } from '@/hooks/useSidebar';
import { Sidebar } from './Sidebar';
import { TopBar } from './TopBar';

interface DashboardLayoutProps {
  children: React.ReactNode;
}

export function DashboardLayout({ children }: DashboardLayoutProps) {
  const sidebarValue = useSidebarProvider();

  return (
    <SidebarContext.Provider value={sidebarValue}>
      <Sidebar />
      <ContentArea>{children}</ContentArea>
    </SidebarContext.Provider>
  );
}

function ContentArea({ children }: { children: React.ReactNode }) {
  const { collapsed } = useSidebar();

  return (
    <div
      className="lg-sidebar-offset"
      style={{ '--sidebar-width': collapsed ? '4rem' : '16rem' } as React.CSSProperties}
    >
      <TopBar />
      <main className="px-4 py-6 sm:px-6 lg:px-8">
        {children}
      </main>
    </div>
  );
}

import { SidebarContext, useSidebarProvider } from '@/hooks/useSidebar';
import { useSidebar } from '@/hooks/useSidebar';
import { Sidebar } from './Sidebar';
import { TopBar } from './TopBar';

interface DashboardLayoutProps {
  title?: string;
  children: React.ReactNode;
}

export function DashboardLayout({ title, children }: DashboardLayoutProps) {
  const sidebarValue = useSidebarProvider();

  return (
    <SidebarContext.Provider value={sidebarValue}>
      <Sidebar />
      <ContentArea title={title}>{children}</ContentArea>
    </SidebarContext.Provider>
  );
}

function ContentArea({ title, children }: { title?: string; children: React.ReactNode }) {
  const { collapsed } = useSidebar();

  return (
    <div
      className="lg-sidebar-offset"
      style={{ '--sidebar-width': collapsed ? '4rem' : '16rem' } as React.CSSProperties}
    >
      <div className="content-inset">
        <TopBar title={title} />
        <main className="px-4 py-6 sm:px-6 lg:px-8">
          {title && (
            <h1 className="mb-4 text-xl font-semibold text-text-primary lg:hidden">
              {title}
            </h1>
          )}
          {children}
        </main>
      </div>
    </div>
  );
}

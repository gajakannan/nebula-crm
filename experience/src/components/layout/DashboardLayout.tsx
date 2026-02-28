import { useCallback, useEffect, useState } from 'react';
import { SidebarContext, useSidebarProvider } from '@/hooks/useSidebar';
import { useSidebar } from '@/hooks/useSidebar';
import { RightChatPanel } from './RightChatPanel';
import { Sidebar } from './Sidebar';
import { TopBar } from './TopBar';

interface DashboardLayoutProps {
  title?: string;
  children: React.ReactNode;
}

export function DashboardLayout({ title, children }: DashboardLayoutProps) {
  const sidebarValue = useSidebarProvider();
  const [chatCollapsed, setChatCollapsed] = useState(() => {
    return localStorage.getItem('nebula-chat-panel-collapsed') === 'true';
  });
  const [chatFullscreen, setChatFullscreen] = useState(false);

  const toggleChatCollapsed = useCallback(() => {
    setChatCollapsed((prev) => {
      localStorage.setItem('nebula-chat-panel-collapsed', String(!prev));
      return !prev;
    });
  }, []);

  const toggleChatFullscreen = useCallback(() => {
    setChatFullscreen((prev) => !prev);
  }, []);

  useEffect(() => {
    function handleResize() {
      if (window.innerWidth < 1024) {
        setChatFullscreen(false);
      }
    }
    window.addEventListener('resize', handleResize);
    return () => window.removeEventListener('resize', handleResize);
  }, []);

  return (
    <SidebarContext.Provider value={sidebarValue}>
      {!chatFullscreen && <Sidebar />}
      <RightChatPanel
        collapsed={chatCollapsed}
        fullscreen={chatFullscreen}
        onToggleFullscreen={toggleChatFullscreen}
      />
      {!chatFullscreen && (
        <ContentArea
          title={title}
          chatCollapsed={chatCollapsed}
          onToggleChatCollapsed={toggleChatCollapsed}
        >
          {children}
        </ContentArea>
      )}
    </SidebarContext.Provider>
  );
}

function ContentArea({
  title,
  children,
  chatCollapsed,
  onToggleChatCollapsed,
}: {
  title?: string;
  children: React.ReactNode;
  chatCollapsed: boolean;
  onToggleChatCollapsed: () => void;
}) {
  const { collapsed } = useSidebar();

  return (
    <div
      className="lg-sidebar-offset"
      style={
        {
          '--sidebar-width': collapsed ? '4rem' : '16rem',
          '--chat-panel-width': chatCollapsed ? '4rem' : '22rem',
        } as React.CSSProperties
      }
    >
      <div className="content-inset">
        <TopBar
          title={title}
          chatCollapsed={chatCollapsed}
          onToggleChatCollapsed={onToggleChatCollapsed}
        />
        <main className="px-4 py-6 sm:px-6 lg:pl-6 lg:pr-8">
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

import { screen } from '@testing-library/react'
import { describe, expect, it, vi } from 'vitest'
import DashboardPage from '@/pages/DashboardPage'
import { renderRouteWithProviders } from '@/test-utils/render-app'

const { mockGetUser } = vi.hoisted(() => ({
  mockGetUser: vi.fn(),
}))

mockGetUser.mockResolvedValue({
  expired: false,
  access_token: 'test-token',
  profile: {},
})

vi.mock('@/features/auth/oidcUserManager', () => ({
  oidcUserManager: {
    getUser: mockGetUser,
    events: {
      addUserLoaded: vi.fn(),
      addUserUnloaded: vi.fn(),
      removeUserLoaded: vi.fn(),
      removeUserUnloaded: vi.fn(),
    },
  },
}))

describe('DashboardPage integration', () => {
  it('loads dashboard data through the shared frontend runtime harness', async () => {
    renderRouteWithProviders(<DashboardPage />, {
      route: '/',
      path: '/',
    })

    expect(await screen.findByText('Your opportunities at a glance')).toBeInTheDocument()
    expect(await screen.findByText('Renewal meeting this afternoon')).toBeInTheDocument()
    expect(await screen.findByText('Active Brokers')).toBeInTheDocument()
    expect(await screen.findByRole('button', { name: 'Received stage, 10 opportunities' })).toBeInTheDocument()
    expect(await screen.findByText('Broker appetite notes were refreshed for the west region team.')).toBeInTheDocument()
    expect((await screen.findAllByRole('link', { name: 'Blue Horizon Risk Partners' })).length).toBeGreaterThan(0)
  })
})

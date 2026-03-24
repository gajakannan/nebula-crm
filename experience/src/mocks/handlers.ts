import { http, HttpResponse } from 'msw'
import {
  API_ORIGIN,
  buildBrokerListResponse,
  buildOpportunityBreakdownFixture,
  buildOpportunityItemsFixture,
  dashboardKpisFixture,
  dashboardNudgesFixture,
  dashboardOpportunitiesFixture,
  opportunityOutcomesFixture,
  opportunityAgingFixture,
  submissionFlowFixture,
  taskFixture,
  timelineFixture,
} from './data'

function apiUrl(path: string): string {
  return new URL(path, API_ORIGIN).toString()
}

export const handlers = [
  http.get(apiUrl('/dashboard/kpis'), () => HttpResponse.json(dashboardKpisFixture)),

  http.get(apiUrl('/dashboard/nudges'), () => HttpResponse.json(dashboardNudgesFixture)),

  http.get(apiUrl('/dashboard/opportunities'), () => {
    return HttpResponse.json(dashboardOpportunitiesFixture)
  }),

  http.get(apiUrl('/dashboard/opportunities/flow'), () => {
    return HttpResponse.json(submissionFlowFixture)
  }),

  http.get(apiUrl('/dashboard/opportunities/outcomes'), () => {
    return HttpResponse.json(opportunityOutcomesFixture)
  }),

  http.get(apiUrl('/dashboard/opportunities/aging'), () => {
    return HttpResponse.json(opportunityAgingFixture)
  }),

  http.get(apiUrl('/dashboard/opportunities/:entityType/:status/breakdown'), ({ params, request }) => {
    const url = new URL(request.url)
    const groupBy = url.searchParams.get('groupBy')
    const periodDays = Number(url.searchParams.get('periodDays') ?? '180')

    if (
      typeof params.entityType !== 'string'
      || typeof params.status !== 'string'
      || !groupBy
    ) {
      return HttpResponse.json({ detail: 'Invalid breakdown request' }, { status: 400 })
    }

    return HttpResponse.json(
      buildOpportunityBreakdownFixture(
        params.entityType as 'submission' | 'renewal',
        decodeURIComponent(params.status),
        groupBy as Parameters<typeof buildOpportunityBreakdownFixture>[2],
        periodDays,
      ),
    )
  }),

  http.get(apiUrl('/dashboard/opportunities/:entityType/:status/items'), () => {
    return HttpResponse.json(buildOpportunityItemsFixture())
  }),

  http.get(apiUrl('/dashboard/opportunities/outcomes/:outcomeKey/items'), () => {
    return HttpResponse.json(buildOpportunityItemsFixture())
  }),

  http.get(apiUrl('/my/tasks'), () => HttpResponse.json(taskFixture)),

  http.get(apiUrl('/timeline/events'), () => {
    return HttpResponse.json({
      data: timelineFixture,
      page: 1,
      pageSize: 12,
      totalCount: timelineFixture.length,
      totalPages: 1,
    })
  }),

  http.get(apiUrl('/brokers'), ({ request }) => {
    const url = new URL(request.url)
    return HttpResponse.json(buildBrokerListResponse(url.searchParams))
  }),
]

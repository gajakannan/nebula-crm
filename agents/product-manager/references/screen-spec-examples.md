# Screen Specification Examples

This document provides generic screen specification examples across different domains. Use these as templates when specifying UIs for your specific solution.

---

## Example 1: Task List Screen (Productivity SaaS)

### Screen Overview

**Screen Name:** Task List
**Route:** `/tasks`
**User Persona:** Project Manager (Alex Rivera)
**Feature:** F1 - Task Organization

**Purpose:**
Display all tasks in a filterable, searchable list with status indicators and quick actions.

---

### Layout

```
┌─────────────────────────────────────────────────────────────┐
│  [App Header with Logo, Navigation, User Menu]             │
├─────────────────────────────────────────────────────────────┤
│  Tasks                                     [+ New Task]     │
├─────────────────────────────────────────────────────────────┤
│  [Search: "Search tasks..."]  [Filter: All] [Sort: Due Date]│
├─────────────────────────────────────────────────────────────┤
│  ┌───────────────────────────────────────────────────────┐ │
│  │ ☐ Design landing page                    ⚡ Critical  │ │
│  │   Assigned to: John Doe │ Due: Feb 5, 2026             │ │
│  │   [View] [Edit] [Delete]                              │ │
│  └───────────────────────────────────────────────────────┘ │
│  ┌───────────────────────────────────────────────────────┐ │
│  │ ☑ Write API documentation                🔵 High      │ │
│  │   Assigned to: Jane Smith │ Due: Feb 3, 2026          │ │
│  │   [View] [Edit] [Delete]                              │ │
│  └───────────────────────────────────────────────────────┘ │
│  ┌───────────────────────────────────────────────────────┐ │
│  │ ☐ Update user guide                      🟢 Medium    │ │
│  │   Assigned to: Bob Jones │ Due: Feb 10, 2026          │ │
│  │   [View] [Edit] [Delete]                              │ │
│  └───────────────────────────────────────────────────────┘ │
└─────────────────────────────────────────────────────────────┘
```

---

### Components

#### 1. Page Header
- **Title:** "Tasks" (H1)
- **Primary Action:** "+ New Task" button (navigates to /tasks/new)

#### 2. Filter Bar
- **Search Input:**
  - Placeholder: "Search tasks..."
  - Searches: Title, Description
  - Debounce: 300ms
  - Clear button (X) when text entered

- **Filter Dropdown:**
  - Options: All, To Do, In Progress, Done, Blocked
  - Default: All
  - Multi-select enabled

- **Sort Dropdown:**
  - Options: Due Date (asc/desc), Priority (High→Low), Created Date, Status
  - Default: Due Date (ascending)

#### 3. Task List
- **Empty State:** "No tasks found. Create your first task to get started."
- **Task Card:**
  - Checkbox (mark complete/incomplete)
  - Title (clickable, navigates to task detail)
  - Priority indicator (⚡ Critical, 🔵 High, 🟢 Medium, ⚪ Low)
  - Assignee name
  - Due date
  - Action buttons: View, Edit, Delete

---

### Interactions

**Click Task Title:**
- Navigate to `/tasks/:id` (Task Detail page)

**Click Checkbox:**
- Toggle task status (To Do ↔ Done)
- Show toast: "Task marked as [done/incomplete]"
- Update UI immediately (optimistic update)

**Click "+ New Task":**
- Navigate to `/tasks/new` (Create Task form)

**Click "Delete":**
- Show confirmation modal: "Are you sure you want to delete this task? This action cannot be undone."
- On confirm: Delete task, show toast "Task deleted"
- On cancel: Close modal

---

### Data Requirements

**API Endpoint:** `GET /api/tasks`

**Query Parameters:**
- `search` (string, optional)
- `status` (array, optional): ["To Do", "In Progress", "Done", "Blocked"]
- `sortBy` (string, optional): "dueDate", "priority", "createdAt", "status"
- `sortOrder` (string, optional): "asc", "desc"
- `page` (int, optional, default: 1)
- `limit` (int, optional, default: 20)

**Response:**
```json
{
  "data": [
    {
      "id": "123e4567-e89b-12d3-a456-426614174000",
      "title": "Design landing page",
      "status": "To Do",
      "priority": "Critical",
      "assignee": {
        "id": "...",
        "name": "John Doe"
      },
      "dueDate": "2026-02-05T00:00:00Z",
      "createdAt": "2026-02-01T10:30:00Z"
    }
  ],
  "pagination": {
    "page": 1,
    "limit": 20,
    "total": 45,
    "totalPages": 3
  }
}
```

---

### Validation Rules

- Search term: Max 100 characters
- Filter: Only valid status values accepted
- Sort: Only valid field names accepted

---

### Error Handling

**API Error (500):**
- Show error banner: "Failed to load tasks. Please try again."
- Show retry button

**Network Error:**
- Show error banner: "Connection lost. Please check your internet connection."

**Empty Search Results:**
- Show message: "No tasks match your search. Try different keywords."

---

### Accessibility

- [ ] Page title set to "Tasks"
- [ ] Search input has `aria-label="Search tasks"`
- [ ] Filter dropdowns have labels
- [ ] Task checkboxes have labels (task title)
- [ ] Keyboard navigation: Tab through search → filters → tasks
- [ ] Enter on task card opens detail view
- [ ] Space on checkbox toggles status

---

### Performance

- [ ] Initial load: < 2 seconds
- [ ] Search debounce: 300ms
- [ ] Pagination: Load 20 tasks at a time
- [ ] Optimistic UI updates for checkbox toggles

---

### Mobile Responsiveness

**Breakpoints:**
- Desktop: > 1024px (3-column layout)
- Tablet: 768px - 1024px (2-column layout)
- Mobile: < 768px (single column, stacked filters)

---

## Example 2: Product Detail Page (E-commerce)

### Screen Overview

**Screen Name:** Product Detail
**Route:** `/products/:id`
**User Persona:** Customer (Maria Santos - shopping for clothing)
**Feature:** F2 - Product Catalog

**Purpose:**
Display product details, images, pricing, and "Add to Cart" functionality.

---

### Layout

```
┌─────────────────────────────────────────────────────────────┐
│  [Header: Logo, Search, Cart Icon (badge: 3)]              │
├─────────────────────────────────────────────────────────────┤
│  ┌───────────────────┐  Product Name (H1)                  │
│  │                   │  ⭐⭐⭐⭐⭐ 4.8 (234 reviews)          │
│  │   Product Image   │                                      │
│  │                   │  $49.99  [was $69.99]                │
│  │   (carousel)      │                                      │
│  └───────────────────┘  Size: [XS] [S] [M] [L] [XL]        │
│                         Color: [●Red] [●Blue] [●Black]      │
│                         Quantity: [-] 1 [+]                 │
│                         [Add to Cart]  [Add to Wishlist]    │
│                                                              │
│  Description                                                │
│  Lorem ipsum product description...                         │
│                                                              │
│  Reviews (234)                                              │
│  ⭐⭐⭐⭐⭐ "Great fit!" - Jane D.                            │
└─────────────────────────────────────────────────────────────┘
```

---

### Components

#### 1. Image Carousel
- Primary image (large, 600x800px)
- Thumbnail strip (4-5 thumbnails)
- Zoom on hover (desktop)
- Swipe gestures (mobile)

#### 2. Product Info
- **Product Name:** H1, 2-line max
- **Rating:** Stars + average + review count
- **Price:**
  - Current price (bold, large)
  - Original price (strikethrough if on sale)
  - Discount badge (if applicable)

#### 3. Variant Selection
- **Size Selector:** Button group (XS, S, M, L, XL)
  - Disabled if out of stock
  - Show "Out of Stock" badge
- **Color Selector:** Color swatches
  - Selected state: border highlight
  - Disabled if out of stock

#### 4. Quantity Selector
- Minus button (disabled if quantity = 1)
- Number input (editable, min: 1, max: stock quantity)
- Plus button (disabled if quantity = stock)

#### 5. Action Buttons
- **Add to Cart:** Primary button
  - Disabled if no size/color selected or out of stock
  - Shows loading spinner when adding
- **Add to Wishlist:** Secondary button

---

### Interactions

**Select Size/Color:**
- Update selected variant
- Fetch stock quantity for variant
- Update "Add to Cart" button state
- Update price if variant has different pricing

**Click "Add to Cart":**
- Validate size/color selected
- Add to cart (API call)
- Show success toast: "Added to cart!"
- Animate cart icon badge (+1)
- Keep user on page

**Click Image Thumbnail:**
- Load full image in main carousel
- Highlight selected thumbnail

---

### Validation Rules

- Size and color must be selected before adding to cart
- Quantity must be between 1 and stock quantity
- Cannot add out-of-stock variants

---

### Error Handling

**Product Not Found (404):**
- Show error page: "Product not found"
- Link to homepage

**Out of Stock:**
- Show "Out of Stock" message
- Disable "Add to Cart" button
- Show "Notify When Available" button

**Add to Cart Failure:**
- Show error toast: "Failed to add to cart. Please try again."
- Keep selected size/color/quantity

---

### Accessibility

- [ ] Images have alt text
- [ ] Color swatches have text labels (not just colors)
- [ ] Quantity input has label
- [ ] All buttons keyboard accessible
- [ ] Size/color selection announced to screen readers

---

## How to Use These Examples

1. **Select a domain** close to your solution
2. **Follow the structure**: Overview → Layout → Components → Interactions → Data → Validation → Errors → Accessibility
3. **Use ASCII diagrams** for layout visualization
4. **Be specific** about validation, error handling, accessibility

---

## For Project-Specific Screens

See your project's `planning-mds/examples/screens/` directory for screen specifications specific to your solution.

---

## Version History

**Version 2.0** - 2026-02-01 - Generic examples (separated from solution-specific content)
**Version 1.0** - Initial version

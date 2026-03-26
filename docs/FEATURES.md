# Features Documentation

Comprehensive guide to all features in The Lost Yeti Kitchen & Bar website.

## Table of Contents

- [User-Facing Features](#user-facing-features)
- [Technical Features](#technical-features)
- [Performance Features](#performance-features)
- [SEO Features](#seo-features)
- [Accessibility Features](#accessibility-features)

## User-Facing Features

### 1. Navigation Header

**Description**: Fixed navigation bar with responsive mobile menu.

**Features**:
- Sticky header that stays visible while scrolling
- Smooth scroll to sections
- Mobile hamburger menu with slide-down animation
- Active link highlighting
- "Book a Table" CTA button

**Components**: `HeaderComponent`

**Usage**:
```html
<app-header></app-header>
```

### 2. Hero Section

**Description**: Full-screen hero with video background and call-to-action.

**Features**:
- Autoplay video background
- Gradient overlay for text readability
- Animated text entrance
- Dual CTA buttons (Explore Menu, Reserve Table)
- Scroll indicator animation

**Components**: `HeroComponent`

**Video**: `assets/hero-background.mp4`

### 3. Animated Background

**Description**: Spline 3D interactive background throughout the site.

**Features**:
- Fixed position behind all content
- Non-interactive (pointer-events: none)
- Smooth performance
- Responsive iframe

**Components**: `AnimatedBackgroundComponent`

**Integration**: Spline Design iframe

### 4. Reservation System

**Description**: Interactive table booking form with custom components.

**Features**:
- Custom calendar for date selection
- Time slot dropdown
- Guest count selector
- Phone number input
- Form validation
- Responsive layout

**Components**:
- `ReservationComponent`
- `CustomCalendarComponent`
- `CustomDropdownComponent`

**Data Model**:
```typescript
interface Reservation {
  fullName: string;
  phone: string;
  date: Date | null;
  time: string;
  guests: string;
}
```

### 5. Custom Calendar

**Description**: Date picker component for reservation dates.

**Features**:
- Month navigation (previous/next)
- Disabled past dates
- "Today" quick select button
- Selected date highlighting
- Responsive grid layout

**Usage**:
```html
<app-custom-calendar 
  [selectedDate]="date"
  (dateSelected)="onDateSelected($event)">
</app-custom-calendar>
```

### 6. Custom Dropdown

**Description**: Styled select component with custom options.

**Features**:
- Custom styling matching theme
- Icon support
- Checkmark for selected item
- Click outside to close
- Smooth animations
- Keyboard navigation

**Usage**:
```html
<app-custom-dropdown
  [options]="['Option 1', 'Option 2']"
  [selectedValue]="selected"
  [placeholder]="'Select option'"
  [icon]="'solar:clock-circle-linear'"
  (valueSelected)="onSelect($event)">
</app-custom-dropdown>
```

### 7. About Section

**Description**: Restaurant story and mission statement.

**Features**:
- Image showcase
- Brand story text
- Responsive layout
- Elegant typography

**Components**: `AboutComponent`

### 8. Category Grid

**Description**: Visual showcase of food categories.

**Features**:
- Grid layout (responsive)
- Category images
- Hover effects
- Category names

**Components**: `CategoryComponent`

### 9. Menu Display

**Description**: Signature dishes with prices and descriptions.

**Features**:
- Dotted line between name and price
- Hover effects
- Responsive typography
- "View Full Menu" CTA

**Components**: `MenuComponent`

**Data Structure**:
```typescript
interface MenuItem {
  name: string;
  price: string;
  description: string;
}
```

### 10. Cinematic Banner

**Description**: Promotional banner with call-to-action.

**Features**:
- Full-width design
- Background image
- Overlay gradient
- CTA button

**Components**: `CinematicBannerComponent`

### 11. Chef Profiles

**Description**: Team showcase with chef information.

**Features**:
- Profile images
- Chef names and titles
- Specialty descriptions
- Grid layout

**Components**: `ChefsComponent`

### 12. Customer Reviews

**Description**: Testimonials from customers.

**Features**:
- Star ratings
- Customer names
- Review text
- Responsive cards

**Components**: `ReviewsComponent`

### 13. Blog Section

**Description**: Latest news and articles.

**Features**:
- Blog post cards
- Featured images
- Post titles and excerpts
- Read more links
- Date display

**Components**: `BlogComponent`

### 14. Locations

**Description**: Restaurant locations and hours.

**Features**:
- Location cards
- Address information
- Opening hours
- Contact details
- Map integration ready

**Components**: `LocationsComponent`

### 15. Footer

**Description**: Site footer with links and information.

**Features**:
- Multi-column layout
- Quick links
- Contact information
- Social media links
- Copyright notice
- Responsive design

**Components**: `FooterComponent`

## Technical Features

### 1. Standalone Components

**Description**: Angular 17 standalone architecture without NgModules.

**Benefits**:
- Simpler component structure
- Better tree-shaking
- Faster compilation
- Easier testing

### 2. TypeScript Strict Mode

**Description**: Full TypeScript type safety.

**Benefits**:
- Catch errors at compile time
- Better IDE support
- Self-documenting code
- Refactoring safety

### 3. Two-Way Data Binding

**Description**: Reactive form handling with `[(ngModel)]`.

**Usage**:
```typescript
[(ngModel)]="reservation.fullName"
```

### 4. Custom SCSS/CSS

**Description**: Component-scoped styles with global utilities.

**Features**:
- Scoped component styles
- Global theme variables
- Custom scrollbar
- Smooth animations

### 5. Lazy Loading

**Description**: Code splitting for optimal performance.

**Implementation**: Route-based lazy loading ready

### 6. Service Workers

**Description**: PWA capabilities (ready for implementation).

**Features**:
- Offline support
- Caching strategies
- Background sync

## Performance Features

### 1. Optimized Assets

**Features**:
- Compressed images
- Optimized video
- Minified CSS/JS
- Tree-shaking

### 2. DNS Prefetch

**Description**: Preconnect to external domains.

```html
<link rel="dns-prefetch" href="https://fonts.googleapis.com">
```

### 3. Deferred Scripts

**Description**: Non-blocking script loading.

```html
<script src="..." defer></script>
```

### 4. Production Build Optimizations

**Features**:
- AOT compilation
- Build optimizer
- Minification
- Source map generation
- Bundle size budgets

## SEO Features

### 1. Meta Tags

**Implemented**:
- Title and description
- Keywords
- Author
- Canonical URL
- Theme color

### 2. Open Graph

**Description**: Social media sharing optimization.

**Tags**:
- og:type
- og:url
- og:title
- og:description
- og:image

### 3. Twitter Cards

**Description**: Twitter-specific meta tags.

**Tags**:
- twitter:card
- twitter:title
- twitter:description
- twitter:image

### 4. Structured Data

**Description**: Schema.org JSON-LD markup.

**Schemas**:
- Restaurant
- LocalBusiness
- Opening hours
- Address
- Contact info

### 5. Sitemap

**File**: `sitemap.xml`

**Includes**:
- All main pages
- Priority levels
- Change frequency
- Last modified dates

### 6. Robots.txt

**File**: `robots.txt`

**Configuration**:
- Allow all crawlers
- Sitemap location
- Crawl delay

## Accessibility Features

### 1. Semantic HTML

**Description**: Proper HTML5 elements.

**Elements**:
- `<header>`, `<nav>`, `<main>`, `<section>`, `<footer>`
- `<button>` for interactive elements
- `<a>` for links

### 2. ARIA Labels

**Description**: Screen reader support.

**Implementation**:
```html
<button aria-label="Close menu">
  <iconify-icon icon="close"></iconify-icon>
</button>
```

### 3. Keyboard Navigation

**Features**:
- Tab navigation
- Enter/Space for buttons
- Escape to close modals
- Focus management

### 4. Focus Indicators

**Description**: Visible focus states for keyboard users.

**Implementation**:
```css
:focus {
  outline: 2px solid #C65A1E;
}
```

### 5. Color Contrast

**Description**: WCAG AA compliant contrast ratios.

**Colors**:
- Text: #ffffff on #0a0a0a (21:1)
- Accent: #C65A1E with proper contrast

### 6. Responsive Text

**Description**: Scalable font sizes.

**Implementation**:
- Base 16px
- Relative units (rem, em)
- Responsive breakpoints

## Future Features

### Planned Additions

1. **Multi-language Support** (i18n)
2. **Online Ordering System**
3. **Payment Integration**
4. **User Authentication**
5. **Admin Dashboard**
6. **Real-time Availability**
7. **Email Notifications**
8. **SMS Confirmations**
9. **Loyalty Program**
10. **Gift Cards**
11. **Event Booking**
12. **Catering Services**
13. **Photo Gallery**
14. **Virtual Tour**
15. **Live Chat**

### Enhancement Ideas

- Progressive Web App (PWA)
- Push notifications
- Offline mode
- Google Maps integration
- Social media feed
- Newsletter subscription
- Analytics dashboard
- A/B testing
- Performance monitoring

---

For implementation details, see [STRUCTURE.md](./STRUCTURE.md) and [TECHNOLOGIES.md](./TECHNOLOGIES.md).

# Styles Guide

Styling guidelines and conventions for The Lost Yeti Kitchen & Bar.

## Design System

### Color Palette

```css
/* Primary Colors */
--bg-primary: #0a0a0a;      /* Main background */
--bg-secondary: #111;        /* Card backgrounds */
--accent: #C65A1E;          /* Orange accent */

/* Text Colors */
--text-primary: #ffffff;     /* Main text */
--text-secondary: #e5e5e5;   /* Secondary text */
--text-muted: #a3a3a3;      /* Muted text */

/* Border Colors */
--border: rgba(255,255,255,0.05);  /* Subtle borders */
```

### Typography

**Fonts**:
- **Headings**: Forum (serif)
- **Body**: Open Sans (sans-serif)

**Font Sizes**:
```css
text-xs: 0.75rem;    /* 12px */
text-sm: 0.875rem;   /* 14px */
text-base: 1rem;     /* 16px */
text-lg: 1.125rem;   /* 18px */
text-xl: 1.25rem;    /* 20px */
text-2xl: 1.5rem;    /* 24px */
text-3xl: 1.875rem;  /* 30px */
text-4xl: 2.25rem;   /* 36px */
text-5xl: 3rem;      /* 48px */
```

### Spacing

```css
gap-1: 0.25rem;   /* 4px */
gap-2: 0.5rem;    /* 8px */
gap-4: 1rem;      /* 16px */
gap-6: 1.5rem;    /* 24px */
gap-8: 2rem;      /* 32px */
gap-12: 3rem;     /* 48px */
```

## Tailwind CSS

### Utility Classes

**Layout**:
```html
<div class="flex flex-col md:flex-row gap-4">
  <!-- Responsive flexbox -->
</div>
```

**Responsive Design**:
```html
<div class="text-sm md:text-base lg:text-lg">
  <!-- Responsive text -->
</div>
```

**Hover Effects**:
```html
<button class="hover:bg-[#C65A1E] transition-all duration-300">
  <!-- Smooth hover -->
</button>
```

### Custom Classes

**Animations**:
```css
.animate-fade-in {
  animation: fadeIn 1s ease-out forwards;
}

@keyframes fadeIn {
  from {
    opacity: 0;
    transform: translateY(20px);
  }
  to {
    opacity: 1;
    transform: translateY(0);
  }
}
```

## Component Styles

### Buttons

```html
<!-- Primary Button -->
<button class="px-8 py-3.5 text-sm font-medium text-white bg-[#C65A1E] rounded-xl hover:bg-[#a84915] transition-all duration-300">
  Button Text
</button>

<!-- Secondary Button -->
<button class="px-8 py-3.5 text-sm font-medium text-white border border-white/20 rounded-xl hover:bg-white/5 transition-all duration-300">
  Button Text
</button>
```

### Cards

```html
<div class="bg-[#111] border border-white/5 rounded-2xl p-8 hover:border-white/10 transition-all">
  <!-- Card content -->
</div>
```

### Forms

```html
<input 
  type="text"
  class="w-full bg-[#0a0a0a] border border-white/10 rounded-xl px-4 py-3 text-sm text-white placeholder-neutral-600 focus:outline-none focus:border-[#C65A1E] focus:ring-1 focus:ring-[#C65A1E] transition-all"
  placeholder="Enter text"
/>
```

## Responsive Breakpoints

```css
sm: 640px   /* Mobile landscape */
md: 768px   /* Tablet */
lg: 1024px  /* Desktop */
xl: 1280px  /* Large desktop */
2xl: 1536px /* Extra large */
```

## Best Practices

1. **Mobile-first**: Start with mobile styles, add larger breakpoints
2. **Consistent spacing**: Use Tailwind spacing scale
3. **Reusable classes**: Create utility classes for common patterns
4. **Performance**: Minimize custom CSS
5. **Accessibility**: Maintain color contrast ratios

## Dark Theme

All components use dark theme by default:
- Dark backgrounds (#0a0a0a, #111)
- Light text (#ffffff, #e5e5e5)
- Orange accents (#C65A1E)
- Subtle borders (white/5, white/10)

---

For more details, see [STRUCTURE.md](./STRUCTURE.md)

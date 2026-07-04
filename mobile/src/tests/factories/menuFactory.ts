/**
 * Factory functions for menu-related mock data
 */

export interface MenuItem {
  id: string;
  nameEn: string;
  nameAr: string;
  descriptionEn: string;
  descriptionAr: string;
  price: number;
  category: string;
  imageUrl?: string;
  available: boolean;
  rating?: number;
  reviewCount?: number;
}

export interface MenuCategory {
  id: string;
  nameEn: string;
  nameAr: string;
  descriptionEn?: string;
  descriptionAr?: string;
  items: MenuItem[];
}

export const createMockMenuItem = (overrides?: Partial<MenuItem>): MenuItem => {
  return {
    id: 'menu-item-' + Math.random().toString(36).substr(2, 9),
    nameEn: 'Grilled Chicken',
    nameAr: 'الدجاج المشوي',
    descriptionEn: 'Succulent grilled chicken with herbs and spices',
    descriptionAr: 'دجاج مشوي لذيذ بالأعشاب والتوابل',
    price: 12.99,
    category: 'main-course',
    imageUrl: 'https://example.com/image.jpg',
    available: true,
    rating: 4.5,
    reviewCount: 28,
    ...overrides,
  };
};

export const createMockMenuCategory = (overrides?: Partial<MenuCategory>): MenuCategory => {
  return {
    id: 'category-' + Math.random().toString(36).substr(2, 9),
    nameEn: 'Main Courses',
    nameAr: 'الأطباق الرئيسية',
    descriptionEn: 'Our delicious main course selection',
    descriptionAr: 'مجموعتنا من الأطباق الرئيسية اللذيذة',
    items: [
      createMockMenuItem({ nameEn: 'Grilled Chicken' }),
      createMockMenuItem({ nameEn: 'Fish Fillet' }),
      createMockMenuItem({ nameEn: 'Lamb Kebab' }),
    ],
    ...overrides,
  };
};

export const createMockMenuList = (count: number = 3): MenuCategory[] => {
  const categories = ['Main Courses', 'Appetizers', 'Desserts', 'Beverages'];
  
  return categories.slice(0, count).map((categoryName, index) =>
    createMockMenuCategory({
      nameEn: categoryName,
      id: `category-${index}`,
    })
  );
};

import { http, HttpResponse } from 'msw';
import { createMockMenuList, createMockMenuCategory } from '../factories/menuFactory';

const API_BASE = process.env.EXPO_PUBLIC_API_URL || 'http://localhost:3000';

/**
 * Menu API mock handlers
 */
export const menuHandlers = [
  // GET /api/menu
  http.get(`${API_BASE}/api/menu`, () => {
    const categories = createMockMenuList(3);

    return HttpResponse.json(
      {
        data: categories,
        total: categories.length,
      },
      { status: 200 }
    );
  }),

  // GET /api/menu/:categoryId
  http.get(`${API_BASE}/api/menu/:categoryId`, ({ params }) => {
    const { categoryId } = params;

    if (!categoryId || categoryId === 'invalid') {
      return HttpResponse.json(
        { error: 'Category not found' },
        { status: 404 }
      );
    }

    const category = createMockMenuCategory({ id: categoryId as string });

    return HttpResponse.json(category, { status: 200 });
  }),

  // GET /api/menu/search
  http.get(`${API_BASE}/api/menu/search`, ({ request }) => {
    const url = new URL(request.url);
    const query = url.searchParams.get('q');

    if (!query) {
      return HttpResponse.json(
        { error: 'Search query is required' },
        { status: 400 }
      );
    }

    const categories = createMockMenuList(1);

    return HttpResponse.json(
      {
        data: categories,
        query,
        total: categories.length,
      },
      { status: 200 }
    );
  }),
];

import { createRouter, createWebHistory } from 'vue-router'

const routes = [
  {
    path: '/',
    name: 'hailongHome',
    component: () => import('@/views/pages/Home.vue')
  },
  {
    path: '/about',
    name: 'About',
    component: () => import('@/views/pages/About.vue')
  },
  {
    path: '/announcements',
    name: 'Announcements',
    component: () => import('@/views/announcements/Announcements.vue')
  },
  {
    path: '/announcement/:id',
    name: 'AnnouncementDetail',
    component: () => import('@/views/details/AnnouncementDetail.vue')
  },
  {
    path: '/news',
    name: 'News',
    component: () => import('@/views/news/News.vue')
  },
  {
    path: '/news/:id',
    name: 'NewsDetail',
    component: () => import('@/views/news/NewsDetail.vue')
  },
  {
    path: '/business-scope/:id',
    name: 'BusinessScopeDetail',
    component: () => import('@/views/details/BusinessScopeDetail.vue')
  },
  {
    path: '/achievement/:id',
    name: 'AchievementDetail',
    component: () => import('@/views/details/AchievementDetail.vue')
  },
  {
    path: '/qualification/:id',
    name: 'QualificationDetail',
    component: () => import('@/views/details/QualificationDetail.vue')
  },
  {
    path: '/policies',
    name: 'Policies',
    component: () => import('@/views/info-publish/Policies.vue')
  },
  {
    path: '/policy/:id',
    name: 'PolicyDetail',
    component: () => import('@/views/details/PolicyDetail.vue')
  },
  {
    path: '/tools',
    name: 'Tools',
    component: () => import('@/views/pages/Tools.vue')
  },
  {
    path: '/contact',
    name: 'Contact',
    component: () => import('@/views/pages/Contact.vue')
  },
  {
    path: '/expert-database',
    name: 'ExpertDatabase',
    component: () => import('@/views/pages/ExpertDatabase.vue')
  }
]

const router = createRouter({
  history: createWebHistory(),
  routes,
  scrollBehavior(to, from, savedPosition) {
    if (savedPosition) {
      return savedPosition
    } else {
      return { top: 0 }
    }
  }
})

export default router

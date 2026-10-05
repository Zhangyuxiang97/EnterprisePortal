import { createRouter, createWebHistory } from 'vue-router'

const routes = [
  {
    path: '/',
    name: 'hailongHome',
    meta: { title: '', navPath: '/' },
    component: () => import('@/views/home/Home.vue')
  },
  {
    path: '/about',
    name: 'About',
    meta: { title: '关于海隆', navPath: '/about' },
    component: () => import('@/views/company/About.vue')
  },
  {
    path: '/announcements',
    name: 'Announcements',
    meta: { title: '公告信息', navPath: '/announcements' },
    component: () => import('@/views/announcements/Announcements.vue')
  },
  {
    path: '/announcement/:id',
    name: 'AnnouncementDetail',
    meta: { title: '公告详情', navPath: '/announcements' },
    component: () => import('@/views/announcements/AnnouncementDetail.vue')
  },
  {
    path: '/news',
    name: 'News',
    meta: { title: '新闻中心', navPath: '/news' },
    component: () => import('@/views/news/News.vue')
  },
  {
    path: '/news/:id',
    name: 'NewsDetail',
    meta: { title: '新闻详情', navPath: '/news' },
    component: () => import('@/views/news/NewsDetail.vue')
  },
  {
    path: '/business-scope/:id',
    name: 'BusinessScopeDetail',
    meta: { title: '业务范围', navPath: '/about' },
    component: () => import('@/views/company/BusinessScopeDetail.vue')
  },
  {
    path: '/achievement/:id',
    name: 'AchievementDetail',
    meta: { title: '重要业绩', navPath: '/about' },
    component: () => import('@/views/company/AchievementDetail.vue')
  },
  {
    path: '/qualification/:id',
    name: 'QualificationDetail',
    meta: { title: '企业资质', navPath: '/about' },
    component: () => import('@/views/company/QualificationDetail.vue')
  },
  {
    path: '/policies',
    name: 'Policies',
    meta: { title: '政策法规', navPath: '/policies' },
    component: () => import('@/views/policies/Policies.vue')
  },
  {
    path: '/policy/:id',
    name: 'PolicyDetail',
    meta: { title: '政策详情', navPath: '/policies' },
    component: () => import('@/views/policies/PolicyDetail.vue')
  },
  {
    path: '/tools',
    name: 'Tools',
    meta: { title: '实用工具', navPath: '/tools' },
    component: () => import('@/views/services/Tools.vue')
  },
  {
    path: '/contact',
    name: 'Contact',
    meta: { title: '联系我们', navPath: '/contact' },
    component: () => import('@/views/company/Contact.vue')
  },
  {
    path: '/expert-database',
    name: 'ExpertDatabase',
    meta: { title: '专家库', navPath: '/expert-database' },
    component: () => import('@/views/services/ExpertDatabase.vue')
  },
  {
    path: '/:pathMatch(.*)*',
    name: 'NotFound',
    component: () => import('@/views/NotFound.vue'),
    meta: { title: '页面不存在', noindex: true }
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

import { session } from '@/utils/session'
import { useUserStore } from '@/stores/user'
import { createRouter, createWebHistory } from 'vue-router'
import { tokenUtils } from '@/utils/auth'

// 路由配置
const routes = [
  {
    path: '/login',
    name: 'Login',
    component: () => import('@/views/Login.vue'),
    meta: { title: '登录' }
  },
  {
    path: '/',
    component: () => import('@/views/Layout.vue'),
    redirect: '/dashboard',
    children: [
      {
        path: '/dashboard',
        name: 'Dashboard',
        component: () => import('@/views/Dashboard.vue'),
        meta: { title: '数据看板', icon: 'DataAnalysis', roles: ['admin', 'user'] }
      },
      {
        path: '/gov-procurement',
        name: 'GovProcurement',
        component: () => import('@/views/announcements/GovProcurement.vue'),
        meta: { title: '政府采购公告', icon: 'Document', roles: ['admin', 'user'] }
      },
      {
        path: '/construction',
        name: 'Construction',
        component: () => import('@/views/announcements/Construction.vue'),
        meta: { title: '建设工程公告', icon: 'Document', roles: ['admin', 'user'] }
      },
      {
        path: '/info-publish',
        name: 'InfoPublish',
        redirect: '/info-publish/news-center',
        meta: { title: '信息发布', icon: 'DocumentCopy', roles: ['admin', 'user'] },
        children: [
          {
            path: '/info-publish/news-center',
            name: 'NewsCenter',
            component: () => import('@/views/info-publish/NewsCenter.vue'),
            meta: { title: '新闻中心', roles: ['admin', 'user'] }
          },
          {
            path: '/info-publish/policy-regulation',
            name: 'PolicyRegulation',
            component: () => import('@/views/info-publish/PolicyRegulation.vue'),
            meta: { title: '政策法规', roles: ['admin', 'user'] }
          }
        ]
      },
      {
        path: '/attachments',
        name: 'Attachments',
        component: () => import('@/views/attachments/AttachmentList.vue'),
        meta: { title: '附件管理', icon: 'FolderOpened', roles: ['admin'] }
      },
      {
        path: '/statistics',
        name: 'Statistics',
        redirect: '/statistics/overview',
        meta: { title: '统计分析', icon: 'DataLine', roles: ['admin'] },
        children: [
          {
            path: '/statistics/overview',
            name: 'SystemOverview',
            component: () => import('@/views/statistics/SystemOverview.vue'),
            meta: { title: '系统概览', roles: ['admin'] }
          },
          {
            path: '/statistics/visit',
            name: 'VisitStatistics',
            component: () => import('@/views/statistics/VisitStatistics.vue'),
            meta: { title: '访问统计', roles: ['admin'] }
          },
          {
            path: '/statistics/announcement',
            name: 'AnnouncementStatistics',
            component: () => import('@/views/statistics/AnnouncementStatistics.vue'),
            meta: { title: '公告统计', roles: ['admin'] }
          },
          {
            path: '/statistics/info-publication',
            name: 'InfoPublicationStatistics',
            component: () => import('@/views/statistics/InfoPublicationStatistics.vue'),
            meta: { title: '信息发布统计', roles: ['admin'] }
          }
        ]
      },
      {
        path: '/config/site-settings',
        name: 'SiteSettings',
        component: () => import('@/views/config/SiteSettings.vue'),
        meta: { title: '门户站点设置', roles: ['admin'] }
      },
      {
        path: '/config',
        name: 'Config',
        redirect: '/config/company-profile',
        meta: { title: '企业资料', icon: 'Setting', roles: ['admin'] },
        children: [
          // 轮播图管理 - 已暂时注释，不对接后台
          // {
          //   path: '/config/banners',
          //   name: 'Banners',
          //   component: () => import('@/views/config/Banners.vue'),
          //   meta: { title: '轮播图管理', roles: ['admin'] }
          // },

          {
            path: '/config/company-profile',
            name: 'CompanyProfile',
            component: () => import('@/views/config/CompanyProfile.vue'),
            meta: { title: '企业简介', roles: ['admin'] }
          },
          {
            path: '/config/business-scope',
            name: 'BusinessScope',
            component: () => import('@/views/config/BusinessScope.vue'),
            meta: { title: '业务范围', roles: ['admin'] }
          },
          {
            path: '/config/qualifications',
            name: 'Qualifications',
            component: () => import('@/views/config/Qualifications.vue'),
            meta: { title: '企业资质', roles: ['admin'] }
          },
          {
            path: '/config/honors',
            name: 'Honors',
            component: () => import('@/views/config/Honors.vue'),
            meta: { title: '企业荣誉', roles: ['admin'] }
          },
          {
            path: '/config/achievements',
            name: 'Achievements',
            component: () => import('@/views/config/Achievements.vue'),
            meta: { title: '重要业绩', roles: ['admin'] }
          }
          // 友情链接 - 已暂时注释
          // {
          //   path: '/config/friendly-links',
          //   name: 'FriendlyLinks',
          //   component: () => import('@/views/config/FriendlyLinks.vue'),
          //   meta: { title: '友情链接', roles: ['admin'] }
          // }
        ]
      },
      {
        path: '/system',
        name: 'System',
        redirect: '/system/change-password',
        meta: { title: '系统管理', icon: 'Tools', roles: ['admin', 'user'] },
        children: [
          {
            path: '/system/users',
            name: 'Users',
            component: () => import('@/views/system/Users.vue'),
            meta: { title: '用户管理', roles: ['admin'] }
          },
          {
            path: '/system/logs',
            name: 'SystemLogs',
            component: () => import('@/views/system/SystemLogs.vue'),
            meta: { title: '系统日志', roles: ['admin'] }
          },
          {
            path: '/system/regions',
            name: 'Regions',
            component: () => import('@/views/system/Regions.vue'),
            meta: { title: '地区字典', roles: ['admin'] }
          },
          {
            path: '/system/change-password',
            name: 'ChangePassword',
            component: () => import('@/views/system/ChangePassword.vue'),
            meta: { title: '修改密码', roles: ['admin', 'user'] }
          }
        ]
      }
    ]
  },
  // 兼容旧路由 - 重定向到 dashboard
  {
    path: '/home',
    redirect: '/dashboard'
  }
]

const router = createRouter({
  history: createWebHistory(),
  routes
})

/**
 * 路由守卫 - 权限验证
 */
router.beforeEach(async (to, from) => {
  document.title = to.meta.title ? to.meta.title + ' - 海隆咨询后台管理' : '海隆咨询后台管理系统'
  if (session.token && !session.verified) {
    try { await useUserStore().getCurrentUser() }
    catch {
      if (session.token) return from.matched.length ? false : (to.path === '/login' ? true : '/login')
    }
  }
  if (to.path === '/login') return session.token && session.verified ? '/dashboard' : true
  if (!session.token || !session.verified) return '/login'
  if (to.meta.roles && !to.meta.roles.includes(session.user?.role)) return '/dashboard'
  return true
})

export default router

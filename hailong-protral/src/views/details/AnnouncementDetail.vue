<template>
  <div class="min-h-screen bg-slate-50/50">
    <Header />
    
    <!-- 顶部悬浮滚动进度条 -->
    <div class="fixed top-0 left-0 w-full h-1 bg-transparent z-50 pointer-events-none">
      <div
        class="h-full bg-gradient-to-r from-hailong-primary to-hailong-secondary transition-all duration-75"
        :style="{ width: scrollProgress + '%' }"
      ></div>
    </div>

    <!-- 面包屑导航 -->
    <div class="bg-white border-b border-slate-100">
      <div class="container-wide py-4">
        <div class="flex items-center text-sm text-slate-500">
          <router-link to="/" class="hover:text-hailong-primary transition-colors flex items-center gap-1">
            <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M3 12l2-2m0 0l7-7 7 7M5 10v10a1 1 0 001 1h3m10-11l2 2m-2-2v10a1 1 0 01-1 1h-3m-6 0a1 1 0 001-1v-4a1 1 0 011-1h2a1 1 0 011 1v4a1 1 0 001 1m-6 0h6" />
            </svg>
            首页
          </router-link>
          <svg class="w-4 h-4 mx-2 text-slate-300" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 5l7 7-7 7" />
          </svg>
          <router-link to="/announcements" class="hover:text-hailong-primary transition-colors">公告信息</router-link>
          <svg class="w-4 h-4 mx-2 text-slate-300" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 5l7 7-7 7" />
          </svg>
          <span class="text-slate-800 font-medium">公告详情</span>
        </div>
      </div>
    </div>

    <!-- 内容区域 -->
    <div class="py-10">
      <div class="container-wide">
        <!-- 加载中 -->
        <div v-if="loading" class="text-center py-32 bg-white rounded-2xl shadow-sm border border-slate-100">
          <div class="inline-block animate-spin rounded-full h-12 w-12 border-4 border-hailong-primary border-t-transparent"></div>
          <p class="mt-4 text-slate-400 text-sm">正在努力加载公告详情...</p>
        </div>

        <!-- 错误处理 -->
        <div v-else-if="error" class="text-center py-20 bg-white rounded-2xl shadow-sm border border-slate-100 max-w-2xl mx-auto">
          <div class="w-16 h-16 bg-red-50 text-red-500 rounded-full flex items-center justify-center mx-auto mb-4">
            <svg class="w-8 h-8" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 9v2m0 4h.01m-6.938 4h13.856c1.54 0 2.502-1.667 1.732-3L13.732 4c-.77-1.333-2.694-1.333-3.464 0L3.34 16c-.77 1.333.192 3 1.732 3z" />
            </svg>
          </div>
          <p class="text-slate-700 font-medium mb-2">获取信息失败</p>
          <p class="text-slate-400 text-sm mb-6">{{ error }}</p>
          <router-link to="/announcements" class="px-6 py-2.5 bg-hailong-primary text-white rounded-xl shadow-md hover:shadow-lg transition-all text-sm font-medium">返回公告列表</router-link>
        </div>

        <!-- 空数据 -->
        <div v-else-if="!announcement" class="text-center py-20 bg-white rounded-2xl shadow-sm border border-slate-100 max-w-2xl mx-auto">
          <div class="w-16 h-16 bg-slate-50 text-slate-400 rounded-full flex items-center justify-center mx-auto mb-4">
            <svg class="w-8 h-8" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9.172 16.172a4 4 0 015.656 0M9 10h.01M15 10h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z" />
            </svg>
          </div>
          <p class="text-slate-500 mb-6">公告不存在或已被删除</p>
          <router-link to="/announcements" class="px-6 py-2.5 bg-slate-100 hover:bg-slate-200 text-slate-700 rounded-xl transition-all text-sm font-medium">返回列表</router-link>
        </div>

        <!-- 核心内容 -->
        <div v-else class="max-w-7xl mx-auto">
          <!-- 1. 公告标题头部卡片 -->
          <div class="bg-white rounded-2xl border border-slate-100 shadow-[0_8px_30px_rgb(0,0,0,0.02)] p-6 md:p-8 mb-6 transition-all duration-300 hover:shadow-[0_15px_35px_rgba(40,120,255,0.04)]">
            <div class="flex items-center gap-2.5 mb-4">
              <span :class="getTypeStyle(announcement.noticeType)" class="px-3 py-1 rounded-full text-xs font-bold shadow-sm">
                {{ getNoticeTypeLabel(announcement.noticeType) }}
              </span>
              <span v-if="announcement.businessType" class="px-3 py-1 rounded-full text-xs font-semibold bg-slate-100 text-slate-600">
                {{ getBusinessTypeLabel(announcement.businessType) }}
              </span>
            </div>
            
            <h1 class="text-2xl md:text-3xl font-extrabold text-slate-900 leading-snug mb-5">
              {{ announcement.title }}
            </h1>

            <div class="flex flex-wrap items-center gap-y-3 gap-x-6 text-sm text-slate-500 pt-5 border-t border-slate-100">
              <div class="flex items-center gap-1.5">
                <svg class="w-4.5 h-4.5 text-hailong-primary" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M8 7V3m8 4V3m-9 8h10M5 21h14a2 2 0 002-2V7a2 2 0 00-2-2H5a2 2 0 00-2 2v12a2 2 0 002 2z" />
                </svg>
                <span class="font-medium">发布时间：</span>
                <span class="text-slate-800">{{ formatDate(announcement.publishTime) }}</span>
              </div>
              <div class="flex items-center gap-1.5">
                <svg class="w-4.5 h-4.5 text-hailong-primary" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M15 12a3 3 0 11-6 0 3 3 0 016 0z" />
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M2.458 12C3.732 7.943 7.523 5 12 5c4.478 0 8.268 2.943 9.542 7-1.274 4.057-5.064 7-9.542 7-4.477 0-8.268-2.943-9.542-7z" />
                </svg>
                <span class="font-medium">浏览次数：</span>
                <span class="text-slate-800">{{ announcement.viewCount || 0 }} 次</span>
              </div>
            </div>
          </div>

          <!-- 2. 采购生命周期时间轴 -->
          <div class="bg-white/80 backdrop-blur-md rounded-2xl border border-slate-100 shadow-[0_8px_30px_rgb(0,0,0,0.01)] p-6 mb-6 no-print">
            <div class="flex items-center justify-between max-w-4xl mx-auto relative px-4">
              <!-- 进度条线 -->
              <div class="absolute left-10 right-10 top-1/2 -translate-y-1/2 h-1 bg-slate-100 -z-10 rounded-full">
                <div
                  class="h-full bg-gradient-to-r from-hailong-primary to-hailong-secondary transition-all duration-500 rounded-full"
                  :style="{ width: getStepPercent(announcement.noticeType) + '%' }"
                ></div>
              </div>
              
              <!-- 节点1 -->
              <div class="flex flex-col items-center">
                <div :class="[
                  'w-10 h-10 rounded-full flex items-center justify-center font-bold text-sm transition-all duration-300 border-2',
                  ['bidding', 'correction', 'result'].includes(announcement.noticeType)
                    ? 'bg-hailong-primary text-white border-hailong-primary shadow-[0_0_15px_rgba(40,120,255,0.35)]'
                    : 'bg-white text-slate-400 border-slate-200'
                ]">
                  <svg v-if="['correction', 'result'].includes(announcement.noticeType)" class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="3" d="M5 13l4 4L19 7" />
                  </svg>
                  <span v-else>1</span>
                </div>
                <span :class="[
                  'text-xs font-bold mt-2.5 transition-colors',
                  ['bidding', 'correction', 'result'].includes(announcement.noticeType) ? 'text-hailong-primary' : 'text-slate-400'
                ]">招标采购公告</span>
              </div>

              <!-- 节点2 -->
              <div class="flex flex-col items-center">
                <div :class="[
                  'w-10 h-10 rounded-full flex items-center justify-center font-bold text-sm transition-all duration-300 border-2',
                  ['correction', 'result'].includes(announcement.noticeType)
                    ? 'bg-hailong-primary text-white border-hailong-primary shadow-[0_0_15px_rgba(40,120,255,0.35)]'
                    : 'bg-white text-slate-400 border-slate-200'
                ]">
                  <svg v-if="['result'].includes(announcement.noticeType)" class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="3" d="M5 13l4 4L19 7" />
                  </svg>
                  <span v-else>2</span>
                </div>
                <span :class="[
                  'text-xs font-bold mt-2.5 transition-colors',
                  ['correction', 'result'].includes(announcement.noticeType) ? 'text-hailong-primary' : 'text-slate-400'
                ]">澄清更正公告</span>
              </div>

              <!-- 节点3 -->
              <div class="flex flex-col items-center">
                <div :class="[
                  'w-10 h-10 rounded-full flex items-center justify-center font-bold text-sm transition-all duration-300 border-2',
                  announcement.noticeType === 'result'
                    ? 'bg-gradient-to-r from-hailong-secondary to-purple-600 text-white border-transparent shadow-[0_0_15px_rgba(162,89,255,0.35)]'
                    : 'bg-white text-slate-400 border-slate-200'
                ]">
                  <span>3</span>
                </div>
                <span :class="[
                  'text-xs font-bold mt-2.5 transition-colors',
                  announcement.noticeType === 'result' ? 'text-hailong-secondary' : 'text-slate-400'
                ]">中标结果公示</span>
              </div>
            </div>
          </div>

          <!-- 3. 双栏网格排版 -->
          <div class="lg:grid lg:grid-cols-3 lg:gap-8 items-start">
            <!-- 左侧：公告内容 + 附件 (2/3) -->
            <div class="lg:col-span-2 space-y-6">
              <!-- 正文卡片 -->
              <div class="bg-white rounded-2xl border border-slate-100 shadow-[0_8px_30px_rgb(0,0,0,0.02)] p-6 md:p-10">
                <h2 class="text-lg md:text-xl font-extrabold text-slate-900 mb-6 pb-4 border-b border-slate-100 flex items-center gap-2.5">
                  <span class="w-1.5 h-5.5 bg-gradient-to-b from-hailong-primary to-hailong-secondary rounded-full flex-shrink-0"></span>
                  公告正文内容
                </h2>
                
                <div class="prose prose-slate max-w-none text-slate-700 leading-relaxed font-yahei" v-html="announcement.content"></div>
              </div>

              <!-- 附件下载卡片 -->
              <div v-if="announcement.attachments && announcement.attachments.length > 0" class="bg-white rounded-2xl border border-slate-100 shadow-[0_8px_30px_rgb(0,0,0,0.02)] p-6 md:p-8">
                <h2 class="text-lg md:text-xl font-extrabold text-slate-900 mb-6 pb-4 border-b border-slate-100 flex items-center gap-2.5">
                  <span class="w-1.5 h-5.5 bg-gradient-to-b from-hailong-primary to-hailong-secondary rounded-full flex-shrink-0"></span>
                  相关附件下载
                </h2>

                <div class="grid grid-cols-1 sm:grid-cols-2 gap-4">
                  <div
                    v-for="attachment in announcement.attachments"
                    :key="attachment.id"
                    :class="[
                      'p-4 rounded-xl border transition-all duration-300 flex flex-col justify-between group hover:shadow-md hover:-translate-y-0.5',
                      getFileTypeConfig(attachment.fileName).bgColor
                    ]"
                  >
                    <div class="flex items-start gap-3 mb-4">
                      <div :class="[
                        'w-10 h-10 rounded-lg flex items-center justify-center flex-shrink-0 bg-white shadow-sm border border-slate-100 text-lg transition-transform duration-300 group-hover:scale-105',
                        getFileTypeConfig(attachment.fileName).iconColor
                      ]">
                        <!-- 根据不同类型渲染不同 SVG -->
                        <svg v-if="getFileTypeConfig(attachment.fileName).icon === 'pdf'" class="w-6 h-6" fill="currentColor" viewBox="0 0 20 20">
                          <path fill-rule="evenodd" d="M4 4a2 2 0 012-2h4.586A1 1 0 0112 2.586L15.414 6a1 1 0 01.586.414L15.414 6A1 1 0 0116 6.586V16a2 2 0 01-2 2H6a2 2 0 01-2-2V4zm2 2h4v3a1 1 0 001 1h3v6H6V6z" clip-rule="evenodd" />
                        </svg>
                        <svg v-else-if="getFileTypeConfig(attachment.fileName).icon === 'excel'" class="w-6 h-6" fill="currentColor" viewBox="0 0 20 20">
                          <path fill-rule="evenodd" d="M5 4a3 3 0 00-3 3v6a3 3 0 003 3h10a3 3 0 003-3V7a3 3 0 00-3-3H5zm4 4H7v2h2V8zm2 0h2v2h-2V8zM7 11h2v2H7v-2zm6 0h-2v2h2v-2z" clip-rule="evenodd" />
                        </svg>
                        <svg v-else-if="getFileTypeConfig(attachment.fileName).icon === 'word'" class="w-6 h-6" fill="currentColor" viewBox="0 0 20 20">
                          <path fill-rule="evenodd" d="M4 4a2 2 0 012-2h8a2 2 0 012 2v12a2 2 0 01-2 2H6a2 2 0 01-2-2V4zm2 12h8V4H6v12zm2-8.5a.5.5 0 01.5-.5h5a.5.5 0 010 1h-5a.5.5 0 01-.5-.5zm0 3a.5.5 0 01.5-.5h5a.5.5 0 010 1h-5a.5.5 0 01-.5-.5zm.5 2.5a.5.5 0 000 1h3a.5.5 0 000-1h-3z" clip-rule="evenodd" />
                        </svg>
                        <svg v-else-if="getFileTypeConfig(attachment.fileName).icon === 'zip'" class="w-6 h-6" fill="currentColor" viewBox="0 0 20 20">
                          <path fill-rule="evenodd" d="M4 2a2 2 0 00-2 2v12a2 2 0 002 2h12a2 2 0 002-2V4a2 2 0 00-2-2H4zm5 3H7v2h2V5zm2 0h-2v2h2V5zm-2 4H7v2h2V9zm2 0h-2v2h2V9zm-2 4H7v2h2v-2zm2 0h-2v2h2v-2zm1-8h2v2h-2V5zm2 4h-2v2h2V9zm-2 4h2v2h-2v-2z" clip-rule="evenodd" />
                        </svg>
                        <svg v-else class="w-6 h-6" fill="currentColor" viewBox="0 0 20 20">
                          <path fill-rule="evenodd" d="M4 4a2 2 0 012-2h4.586A1 1 0 0112 2.586L15.414 6A1 1 0 0116 6.586V16a2 2 0 01-2 2H6a2 2 0 01-2-2V4zm2 6a1 1 0 011-1h6a1 1 0 110 2H7a1 1 0 01-1-1zm1 3a1 1 0 100 2h6a1 1 0 100-2H7z" clip-rule="evenodd" />
                        </svg>
                      </div>

                      <div class="flex-1 min-w-0">
                        <div class="text-slate-800 font-bold text-sm truncate group-hover:text-hailong-primary transition-colors mb-1" :title="attachment.fileName">
                          {{ attachment.fileName }}
                        </div>
                        <div class="text-xs text-slate-400">
                          文件大小：{{ formatFileSize(attachment.fileSize) }}
                        </div>
                      </div>
                    </div>

                    <div class="flex items-center gap-2">
                      <button
                        @click="handlePreview(attachment)"
                        class="flex-1 px-3 py-2 bg-white border border-slate-200 text-slate-600 rounded-lg hover:text-hailong-primary hover:border-hailong-primary/30 text-xs font-semibold transition-all duration-300 flex items-center justify-center gap-1"
                      >
                        <svg class="w-3.5 h-3.5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                          <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M15 12a3 3 0 11-6 0 3 3 0 016 0z" />
                          <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M2.458 12C3.732 7.943 7.523 5 12 5c4.478 0 8.268 2.943 9.542 7-1.274 4.057-5.064 7-9.542 7-4.477 0-8.268-2.943-9.542-7z" />
                        </svg>
                        预览
                      </button>
                      
                      <a
                        :href="attachment.fileUrl"
                        download
                        class="flex-1 px-3 py-2 bg-gradient-to-r from-hailong-primary to-hailong-secondary text-white rounded-lg hover:shadow-md text-xs font-semibold transition-all duration-300 flex items-center justify-center gap-1"
                      >
                        <svg class="w-3.5 h-3.5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                          <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M4 16v1a3 3 0 003 3h10a3 3 0 003-3v-1m-4-4l-4 4m0 0l-4-4m4 4V4" />
                        </svg>
                        下载
                      </a>
                    </div>
                  </div>
                </div>
              </div>
            </div>

            <!-- 右侧：侧边栏商务核心参数 (1/3) -->
            <div class="lg:col-span-1 mt-6 lg:mt-0 lg:sticky lg:top-24 space-y-6 no-print">
              <!-- 项目基本信息面板 -->
              <div class="bg-white rounded-2xl border border-slate-100 shadow-[0_10px_35px_rgba(40,120,255,0.03)] p-6">
                <h3 class="text-base font-extrabold text-slate-800 mb-5 pb-3.5 border-b border-slate-100 flex items-center justify-between">
                  <span class="flex items-center gap-2">
                    <span class="w-1 h-4 bg-hailong-primary rounded-full"></span>
                    关键项目指标
                  </span>
                  <span class="text-[10px] tracking-wider text-slate-300 font-semibold uppercase">Parameters</span>
                </h3>

                <div class="space-y-5">
                  <!-- 预算金额突出 -->
                  <div v-if="announcement.budgetAmount" class="p-4 bg-slate-50 rounded-xl border border-slate-100">
                    <div class="text-[11px] text-slate-400 font-semibold mb-1">招标项目预算金额</div>
                    <div class="flex items-baseline gap-0.5 text-hailong-primary font-tech">
                      <span class="text-lg font-bold">¥</span>
                      <span class="text-3xl font-extrabold tracking-tight">{{ announcement.budgetAmount }}</span>
                      <span class="text-sm font-bold ml-1 text-slate-600">万元</span>
                    </div>
                  </div>

                  <!-- 招标单位 -->
                  <div v-if="announcement.bidder" class="flex items-start gap-3">
                    <div class="w-8 h-8 rounded-lg bg-blue-50 text-hailong-primary flex items-center justify-center flex-shrink-0">
                      <svg class="w-4.5 h-4.5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 21V5a2 2 0 00-2-2H7a2 2 0 00-2 2v16m14 0h2m-2 0h-5m-9 0H3m2 0h5M9 7h1m-1 4h1m4-4h1m-1 4h1m-5 10v-5a1 1 0 011-1h2a1 1 0 011 1v5m-4 0h4" />
                      </svg>
                    </div>
                    <div class="min-w-0">
                      <div class="text-[11px] text-slate-400 font-medium">采购招标单位</div>
                      <div class="text-sm font-bold text-slate-800 break-all leading-normal">{{ announcement.bidder }}</div>
                    </div>
                  </div>

                  <!-- 中标人 -->
                  <div v-if="announcement.winner" class="flex items-start gap-3">
                    <div class="w-8 h-8 rounded-lg bg-emerald-50 text-emerald-600 flex items-center justify-center flex-shrink-0">
                      <svg class="w-4.5 h-4.5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 12l2 2 4-4M7.835 4.697a3.42 3.42 0 001.946-.806 3.42 3.42 0 014.438 0 3.42 3.42 0 001.946.806 3.42 3.42 0 013.138 3.138 3.42 3.42 0 00.806 1.946 3.42 3.42 0 010 4.438 3.42 3.42 0 00-.806 1.946 3.42 3.42 0 01-3.138 3.138 3.42 3.42 0 00-1.946.806 3.42 3.42 0 01-4.438 0 3.42 3.42 0 00-1.946-.806 3.42 3.42 0 01-3.138-3.138 3.42 3.42 0 00-.806-1.946 3.42 3.42 0 010-4.438 3.42 3.42 0 00.806-1.946 3.42 3.42 0 013.138-3.138z" />
                      </svg>
                    </div>
                    <div class="min-w-0">
                      <div class="text-[11px] text-slate-400 font-medium">最终中标单位</div>
                      <div class="text-sm font-extrabold text-emerald-600 break-all leading-normal">{{ announcement.winner }}</div>
                    </div>
                  </div>

                  <!-- 区域 -->
                  <div v-if="announcement.province || announcement.city || announcement.district" class="flex items-start gap-3">
                    <div class="w-8 h-8 rounded-lg bg-cyan-50 text-cyan-600 flex items-center justify-center flex-shrink-0">
                      <svg class="w-4.5 h-4.5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M17.657 16.657L13.414 20.9a1.998 1.998 0 01-2.827 0l-4.244-4.243a8 8 0 1111.314 0z" />
                        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M15 11a3 3 0 11-6 0 3 3 0 016 0z" />
                      </svg>
                    </div>
                    <div class="min-w-0">
                      <div class="text-[11px] text-slate-400 font-medium">执行项目区域</div>
                      <div class="text-sm font-bold text-slate-800 leading-normal">{{ getRegionText(announcement) }}</div>
                    </div>
                  </div>
                </div>
              </div>

              <!-- 快捷交互卡片 -->
              <div class="bg-white rounded-2xl border border-slate-100 shadow-[0_8px_30px_rgb(0,0,0,0.02)] p-5">
                <div class="flex flex-col gap-3">
                  <button
                    @click="handlePrint"
                    class="w-full py-3 bg-slate-900 hover:bg-slate-800 text-white rounded-xl font-bold transition-all duration-300 flex items-center justify-center gap-2 hover:-translate-y-0.5 shadow-lg shadow-slate-950/15 text-sm"
                  >
                    <svg class="w-4.5 h-4.5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                      <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M17 17h2a2 2 0 002-2v-4a2 2 0 00-2-2H5a2 2 0 00-2 2v4a2 2 0 002 2h2m2 4h6a2 2 0 002-2v-4a2 2 0 00-2-2H9a2 2 0 00-2 2v4a2 2 0 002 2zm8-12V5a2 2 0 00-2-2H9a2 2 0 00-2 2v4h10z" />
                    </svg>
                    打印此公告正文
                  </button>
                  
                  <button
                    @click="handleShare"
                    class="w-full py-3 bg-white hover:bg-slate-50 text-slate-700 border border-slate-200 rounded-xl font-bold transition-all duration-300 flex items-center justify-center gap-2 hover:-translate-y-0.5 text-sm"
                  >
                    <svg class="w-4.5 h-4.5 text-slate-500" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                      <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M8.684 13.342C8.886 12.938 9 12.482 9 12c0-.482-.114-.938-.316-1.342m0 2.684a3 3 0 110-2.684m0 2.684l6.632 3.316m-6.632-6l6.632-3.316m0 0a3 3 0 105.367-2.684 3 3 0 00-5.367 2.684zm0 9.316a3 3 0 105.368 2.684 3 3 0 00-5.368-2.684z" />
                    </svg>
                    分享并复制链接
                  </button>
                  
                  <button
                    @click="goBack"
                    class="w-full py-3 bg-slate-50 hover:bg-slate-100 text-slate-600 rounded-xl font-bold transition-all duration-300 flex items-center justify-center gap-2 text-sm"
                  >
                    <svg class="w-4.5 h-4.5 text-slate-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                      <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M10 19l-7-7m0 0l7-7m-7 7h18" />
                    </svg>
                    返回上一页
                  </button>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- 自定义 Toast 提示 -->
    <Transition name="toast">
      <div
        v-if="toastVisible"
        class="fixed bottom-10 left-1/2 transform -translate-x-1/2 z-50 bg-slate-900/95 backdrop-blur-md text-white px-6 py-3.5 rounded-2xl shadow-2xl flex items-center gap-2 border border-slate-800 text-sm font-bold animate-fade-in"
      >
        <svg class="w-5 h-5 text-emerald-400 flex-shrink-0" fill="none" stroke="currentColor" viewBox="0 0 24 24">
          <path stroke-linecap="round" stroke-linejoin="round" stroke-width="3" d="M5 13l4 4L19 7" />
        </svg>
        <span>{{ toastMessage }}</span>
      </div>
    </Transition>

    <!-- 附件预览组件 -->
    <AttachmentPreview
      :visible="previewVisible"
      :attachment="currentAttachment"
      @close="previewVisible = false"
    />

    <Footer />
  </div>
</template>

<script setup>
import { ref, onMounted, onUnmounted } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { getAnnouncementDetail } from '@/api/announcement'
import Header from '@/components/Header.vue'
import Footer from '@/components/Footer.vue'
import AttachmentPreview from '@/components/AttachmentPreview.vue'
import { formatDate } from '@/utils/date'

const route = useRoute()
const router = useRouter()

// 公告数据
const announcement = ref(null)
const loading = ref(true)
const error = ref(null)

// 滚动与阅读进度
const scrollProgress = ref(0)
const updateScrollProgress = () => {
  const totalHeight = document.documentElement.scrollHeight - window.innerHeight
  if (totalHeight > 0) {
    scrollProgress.value = (window.scrollY / totalHeight) * 100
  } else {
    scrollProgress.value = 0
  }
}

// 提示反馈
const toastVisible = ref(false)
const toastMessage = ref('')
const showToast = (message) => {
  toastMessage.value = message
  toastVisible.value = true
  setTimeout(() => {
    toastVisible.value = false
  }, 2200)
}

// 分享
const handleShare = () => {
  const url = window.location.href
  navigator.clipboard.writeText(url).then(() => {
    showToast('链接复制成功！可以粘贴分享给他人了。')
  }).catch(err => {
    console.error('复制失败:', err)
    showToast('复制链接失败，请直接在地址栏复制')
  })
}

// 附件预览
const previewVisible = ref(false)
const currentAttachment = ref({})

// 返回上一页
const goBack = () => {
  if (window.history.length <= 1) {
    router.push('/announcements')
  } else {
    router.back()
  }
}

// 获取步骤占比
const getStepPercent = (type) => {
  if (type === 'result') return 100
  if (type === 'correction') return 50
  return 0
}

// 根据文件名动态返回图标、高亮样式
const getFileTypeConfig = (fileName) => {
  if (!fileName) return { iconColor: 'text-slate-400', bgColor: 'bg-slate-50 border-slate-100 hover:bg-slate-100', icon: 'file' }
  const ext = fileName.split('.').pop().toLowerCase()
  switch (ext) {
    case 'pdf':
      return {
        iconColor: 'text-red-500',
        bgColor: 'bg-red-50/30 border-red-100/50 hover:bg-red-50/70',
        icon: 'pdf'
      }
    case 'doc':
    case 'docx':
      return {
        iconColor: 'text-blue-500',
        bgColor: 'bg-blue-50/30 border-blue-100/50 hover:bg-blue-50/70',
        icon: 'word'
      }
    case 'xls':
    case 'xlsx':
      return {
        iconColor: 'text-emerald-500',
        bgColor: 'bg-emerald-50/30 border-emerald-100/50 hover:bg-emerald-50/70',
        icon: 'excel'
      }
    case 'zip':
    case 'rar':
    case '7z':
      return {
        iconColor: 'text-amber-500',
        bgColor: 'bg-amber-50/30 border-amber-100/50 hover:bg-amber-50/70',
        icon: 'zip'
      }
    default:
      return {
        iconColor: 'text-slate-500',
        bgColor: 'bg-slate-50/40 border-slate-100 hover:bg-slate-50',
        icon: 'file'
      }
  }
}

// 获取公告类型标签
const getNoticeTypeLabel = (type) => {
  const typeMap = {
    'bidding': '招标公告',
    'correction': '更正公告',
    'result': '结果公告'
  }
  return typeMap[type] || type
}

// 获取业务类型标签
const getBusinessTypeLabel = (type) => {
  const typeMap = {
    'GOV_PROCUREMENT': '政府采购',
    'CONSTRUCTION': '建设工程'
  }
  return typeMap[type] || type
}

// 获取类型样式
const getTypeStyle = (type) => {
  switch (type) {
    case 'bidding':
      return 'bg-hailong-primary/10 text-hailong-primary border border-hailong-primary/20'
    case 'result':
      return 'bg-hailong-secondary/10 text-hailong-secondary border border-hailong-secondary/20'
    case 'correction':
      return 'bg-hailong-cyan/10 text-hailong-cyan border border-hailong-cyan/20'
    default:
      return 'bg-slate-100 text-slate-600 border border-slate-200'
  }
}

// 获取区域文本
const getRegionText = (announcement) => {
  const parts = []
  if (announcement.province) parts.push(announcement.province)
  if (announcement.city) parts.push(announcement.city)
  if (announcement.district) parts.push(announcement.district)
  return parts.join(' ')
}

// 格式化文件大小
const formatFileSize = (bytes) => {
  if (!bytes || bytes === 0) return '0 B'
  const k = 1024
  const sizes = ['B', 'KB', 'MB', 'GB']
  const i = Math.floor(Math.log(bytes) / Math.log(k))
  return Math.round(bytes / Math.pow(k, i) * 100) / 100 + ' ' + sizes[i]
}

// 预览附件
const handlePreview = (attachment) => {
  currentAttachment.value = attachment
  previewVisible.value = true
}

// 打印公告
const handlePrint = () => {
  window.print()
}

// 加载公告详情
const loadAnnouncementDetail = async () => {
  loading.value = true
  error.value = null
  
  try {
    const id = route.params.id
    if (!id) {
      error.value = '公告ID不存在'
      return
    }
    
    const response = await getAnnouncementDetail(id)
    if (response.success && response.data) {
      announcement.value = response.data
    } else {
      error.value = response.message || '获取公告详情失败'
      announcement.value = null
    }
  } catch (err) {
    console.error('加载公告详情失败:', err)
    error.value = err.message || '加载公告详情失败，请稍后重试'
    announcement.value = null
  } finally {
    loading.value = false
  }
}

onMounted(() => {
  loadAnnouncementDetail()
  window.addEventListener('scroll', updateScrollProgress)
})

onUnmounted(() => {
  window.removeEventListener('scroll', updateScrollProgress)
})
</script>

<style scoped>
.prose :deep(h1), .prose :deep(h2), .prose :deep(h3) {
  color: #0f172a;
  font-weight: 800;
  margin-top: 1.8rem;
  margin-bottom: 0.8rem;
}

.prose :deep(h2) {
  font-size: 1.4rem;
  border-left: 4px solid #2878FF;
  padding-left: 0.6rem;
}

.prose :deep(p) {
  margin-bottom: 1.2rem;
  line-height: 1.8;
  color: #334155;
  font-size: 1.05rem;
}

/* 表格优雅样式 */
.prose :deep(table) {
  width: 100%;
  border-collapse: separate;
  border-spacing: 0;
  margin: 1.8rem 0;
  border: 1px solid #f1f5f9;
  border-radius: 12px;
  overflow: hidden;
  box-shadow: 0 4px 6px -1px rgb(0 0 0 / 0.02), 0 2px 4px -2px rgb(0 0 0 / 0.02);
}

.prose :deep(table th),
.prose :deep(table td) {
  border-bottom: 1px solid #f1f5f9;
  border-right: 1px solid #f1f5f9;
  padding: 1rem 1.2rem;
  text-align: left;
  font-size: 0.95rem;
}

.prose :deep(table th:last-child),
.prose :deep(table td:last-child) {
  border-right: none;
}

.prose :deep(table tr:last-child td) {
  border-bottom: none;
}

.prose :deep(table th) {
  background-color: #f8fafc;
  font-weight: 700;
  color: #1e293b;
}

.prose :deep(table tr:nth-child(even)) {
  background-color: #f8fafc/50;
}

.prose :deep(table tr:hover) {
  background-color: #f1f5f9/60;
}

/* Toast Transition */
.toast-enter-active,
.toast-leave-active {
  transition: all 0.3s ease-out;
}

.toast-enter-from {
  opacity: 0;
  transform: translate(-50%, 20px);
}

.toast-leave-to {
  opacity: 0;
  transform: translate(-50%, -20px);
}

@media print {
  :deep(header), :deep(footer), .no-print {
    display: none !important;
  }
  body, .min-h-screen {
    background: white !important;
    padding: 0 !important;
  }
  .container-wide {
    width: 100% !important;
    max-width: 100% !important;
    padding: 0 !important;
    margin: 0 !important;
  }
  .lg:col-span-2 {
    width: 100% !important;
  }
}
</style>
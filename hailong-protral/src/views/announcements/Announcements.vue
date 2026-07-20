<template>
  <div class="min-h-screen bg-slate-50/50">
    <Header />
    
    <!-- 页面头部 Hero Banner -->
    <div class="relative pt-32 pb-20 text-center text-white overflow-hidden bg-gradient-to-br from-hailong-dark via-slate-900 to-indigo-950">
      <!-- 几何网格与光晕背景 -->
      <div class="absolute inset-0 bg-grid-pattern opacity-10 pointer-events-none"></div>
      <div class="absolute -top-40 left-1/2 -translate-x-1/2 w-[600px] h-[600px] bg-hailong-primary/10 rounded-full blur-[120px] pointer-events-none animate-float"></div>
      
      <div class="relative z-10 max-w-4xl mx-auto px-6">
        <h1 class="text-4xl md:text-5xl font-extrabold mb-4 font-tech tracking-tight bg-gradient-to-r from-white via-slate-100 to-slate-200 bg-clip-text text-transparent">
          公告信息
        </h1>
        <p class="text-base md:text-lg text-slate-300 font-medium max-w-2xl mx-auto mb-6">
          实时发布最权威的政府采购与建设工程招投标动态信息
        </p>
        
        <!-- 数据活跃面板 -->
        <div class="inline-flex flex-wrap items-center justify-center gap-y-2 gap-x-6 md:gap-x-10 bg-white/5 backdrop-blur-md rounded-2xl px-6 py-3 border border-white/10 shadow-lg text-xs md:text-sm">
          <div class="flex items-center gap-2">
            <span class="w-2 h-2 rounded-full bg-emerald-400 animate-pulse"></span>
            <span class="text-slate-300">今日新增：<strong class="text-white font-tech text-sm md:text-base">12</strong> 条</span>
          </div>
          <div class="w-px h-4 bg-white/20 hidden md:block"></div>
          <div class="flex items-center gap-2">
            <span class="w-2 h-2 rounded-full bg-blue-400"></span>
            <span class="text-slate-300">累计公告：<strong class="text-white font-tech text-sm md:text-base">1,842</strong> 条</span>
          </div>
          <div class="w-px h-4 bg-white/20 hidden md:block"></div>
          <div class="flex items-center gap-2">
            <span class="w-2 h-2 rounded-full bg-purple-400"></span>
            <span class="text-slate-300">本月采购总额：<strong class="text-white font-tech text-sm md:text-base">8,450</strong> 万元</span>
          </div>
        </div>
      </div>
    </div>

    <!-- 内容区域 -->
    <div class="py-10">
      <div class="container-wide">
        <div class="animate-fade-in">
          <!-- 搜索筛选控制台 -->
          <div class="bg-white rounded-2xl shadow-[0_8px_30px_rgb(0,0,0,0.02)] border border-slate-100 p-6 mb-6">
            <!-- 关键字主搜索栏 -->
            <div class="mb-5">
              <div class="flex gap-3">
                <div class="relative flex-1 group">
                  <svg class="absolute left-4 top-1/2 transform -translate-y-1/2 w-5 h-5 text-slate-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M21 21l-6-6m2-5a7 7 0 11-14 0 7 7 0 0114 0z" />
                  </svg>
                  <input
                    v-model="searchParams.keyword"
                    type="text"
                    placeholder="请输入项目名称、招标单位、采购代理机构等关键字"
                    class="w-full pl-12 pr-10 py-3 border border-slate-200 rounded-xl focus:ring-2 focus:ring-hailong-primary/20 focus:border-hailong-primary outline-none transition-all text-sm hover:border-slate-300 bg-slate-50/50 focus:bg-white"
                    @keyup.enter="handleSearch"
                  />
                  <!-- 一键清除按钮 -->
                  <button
                    v-if="searchParams.keyword"
                    @click="clearKeyword"
                    class="absolute right-3.5 top-1/2 transform -translate-y-1/2 text-slate-400 hover:text-slate-600 transition-colors focus:outline-none"
                    title="清除关键字"
                  >
                    <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                      <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12" />
                    </svg>
                  </button>
                </div>
                <button
                  @click="handleSearch"
                  class="px-8 py-3 bg-gradient-to-r from-hailong-primary to-hailong-secondary text-white rounded-xl hover:shadow-lg transition-all font-bold text-sm hover:-translate-y-0.5"
                >
                  搜索
                </button>
                <button
                  @click="handleReset"
                  class="px-6 py-3 bg-slate-100 hover:bg-slate-200 text-slate-600 rounded-xl transition-all font-bold text-sm"
                >
                  重置
                </button>
              </div>
            </div>

            <!-- 详细条件过滤行 -->
            <div class="space-y-4 pt-4 border-t border-slate-100">
              <!-- 1. 业务与采购类型 -->
              <div class="flex items-center gap-4 flex-wrap">
                <span class="text-xs font-bold text-slate-400 whitespace-nowrap w-16 uppercase">业务分类</span>
                <div class="flex flex-wrap gap-2">
                  <button
                    v-for="type in businessTypes"
                    :key="type.value"
                    @click="handleBusinessTypeChange(type.value)"
                    :class="[
                      'px-4 py-1.5 rounded-lg text-xs font-bold transition-all border',
                      searchParams.businessType === type.value
                        ? 'bg-gradient-to-r from-hailong-primary to-hailong-secondary text-white border-transparent shadow-md shadow-blue-500/15'
                        : 'bg-white text-slate-600 border-slate-200 hover:bg-slate-50 hover:-translate-y-0.5'
                    ]"
                  >
                    {{ type.label }}
                  </button>
                </div>

                <!-- 政府采购细分类型 -->
                <template v-if="searchParams.businessType === 'GOV_PROCUREMENT'">
                  <span class="text-xs font-bold text-slate-400 whitespace-nowrap ml-6 uppercase">采购类型</span>
                  <div class="flex flex-wrap gap-2">
                    <button
                      v-for="type in procurementTypes"
                      :key="type.value"
                      @click="handleProcurementTypeChange(type.value)"
                      :class="[
                        'px-4 py-1.5 rounded-lg text-xs font-bold transition-all border',
                        searchParams.procurementType === type.value
                          ? 'bg-gradient-to-r from-hailong-primary to-hailong-secondary text-white border-transparent shadow-md'
                          : 'bg-white text-slate-600 border-slate-200 hover:bg-slate-50'
                      ]"
                    >
                      {{ type.label }}
                    </button>
                  </div>
                </template>
              </div>

              <!-- 2. 公告类型 -->
              <div class="flex items-center gap-4 flex-wrap">
                <span class="text-xs font-bold text-slate-400 whitespace-nowrap w-16 uppercase">公告类型</span>
                <div class="flex flex-wrap gap-2">
                  <button
                    v-for="type in currentAnnouncementTypes"
                    :key="type.value"
                    @click="handleNoticeTypeChange(type.value)"
                    :class="[
                      'px-4 py-1.5 rounded-lg text-xs font-bold transition-all border',
                      searchParams.noticeType === type.value
                        ? 'bg-gradient-to-r from-hailong-primary to-hailong-secondary text-white border-transparent shadow-md shadow-blue-500/15'
                        : 'bg-white text-slate-600 border-slate-200 hover:bg-slate-50 hover:-translate-y-0.5'
                    ]"
                  >
                    {{ type.label }}
                  </button>
                </div>
              </div>

              <!-- 3. 项目区域与发布时间 -->
              <div class="flex items-center gap-4 flex-wrap">
                <span class="text-xs font-bold text-slate-400 whitespace-nowrap w-16 uppercase">执行区域</span>
                
                <div class="flex flex-wrap items-center gap-2">
                  <!-- 省 -->
                  <div class="relative">
                    <select
                      v-model="searchParams.province"
                      @change="onProvinceChange"
                      class="pl-4 pr-9 py-1.5 border border-slate-200 rounded-lg text-xs font-semibold bg-white hover:border-hailong-primary outline-none transition-all cursor-pointer appearance-none min-w-[110px]"
                    >
                      <option value="">全部省份</option>
                      <option v-for="province in provinces" :key="province.regionCode" :value="province.regionName">
                        {{ province.regionName }}
                      </option>
                    </select>
                    <svg class="absolute right-3 top-1/2 transform -translate-y-1/2 w-3.5 h-3.5 text-slate-400 pointer-events-none" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                      <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 9l-7 7-7-7" />
                    </svg>
                  </div>

                  <!-- 市 -->
                  <div class="relative" v-if="searchParams.province">
                    <select
                      v-model="searchParams.city"
                      @change="onCityChange"
                      class="pl-4 pr-9 py-1.5 border border-slate-200 rounded-lg text-xs font-semibold bg-white hover:border-hailong-primary outline-none transition-all cursor-pointer appearance-none min-w-[110px]"
                    >
                      <option value="">全部城市</option>
                      <option v-for="city in cities" :key="city.regionCode" :value="city.regionName">
                        {{ city.regionName }}
                      </option>
                    </select>
                    <svg class="absolute right-3 top-1/2 transform -translate-y-1/2 w-3.5 h-3.5 text-slate-400 pointer-events-none" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                      <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 9l-7 7-7-7" />
                    </svg>
                  </div>

                  <!-- 区 -->
                  <div class="relative" v-if="searchParams.city">
                    <select
                      v-model="searchParams.district"
                      @change="onDistrictChange"
                      class="pl-4 pr-9 py-1.5 border border-slate-200 rounded-lg text-xs font-semibold bg-white hover:border-hailong-primary outline-none transition-all cursor-pointer appearance-none min-w-[110px]"
                    >
                      <option value="">全部区县</option>
                      <option v-for="district in districts" :key="district.regionCode" :value="district.regionName">
                        {{ district.regionName }}
                      </option>
                    </select>
                    <svg class="absolute right-3 top-1/2 transform -translate-y-1/2 w-3.5 h-3.5 text-slate-400 pointer-events-none" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                      <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 9l-7 7-7-7" />
                    </svg>
                  </div>
                </div>

                <span class="text-xs font-bold text-slate-400 whitespace-nowrap ml-6 uppercase">时间范围</span>
                <div class="flex flex-wrap items-center gap-2">
                  <button
                    v-for="time in timeRanges"
                    :key="time.value"
                    @click="selectTimeRange(time.value)"
                    :class="[
                      'px-4 py-1.5 rounded-lg text-xs font-bold transition-all border',
                      searchParams.timeRange === time.value
                        ? 'bg-gradient-to-r from-hailong-primary to-hailong-secondary text-white border-transparent shadow-md shadow-blue-500/15'
                        : 'bg-white text-slate-600 border-slate-200 hover:bg-slate-50'
                    ]"
                  >
                    {{ time.label }}
                  </button>
                  <button
                    @click="showCustomDatePicker = !showCustomDatePicker"
                    :class="[
                      'px-4 py-1.5 rounded-lg text-xs font-bold transition-all flex items-center gap-1 border',
                      searchParams.timeRange === 'custom'
                        ? 'bg-gradient-to-r from-hailong-primary to-hailong-secondary text-white border-transparent shadow-md'
                        : 'bg-white text-slate-600 border-slate-200 hover:bg-slate-50'
                    ]"
                  >
                    <svg class="w-3.5 h-3.5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                      <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M8 7V3m8 4V3m-9 8h10M5 21h14a2 2 0 002-2V7a2 2 0 00-2-2H5a2 2 0 00-2 2v12a2 2 0 002 2z" />
                    </svg>
                    自定义日期
                  </button>

                  <!-- 日期范围选择 -->
                  <template v-if="showCustomDatePicker">
                    <input
                      type="date"
                      v-model="searchParams.startDate"
                      @change="onCustomDateChange"
                      class="px-3 py-1 border border-slate-200 rounded-lg text-xs font-semibold focus:ring-2 focus:ring-hailong-primary/20 outline-none hover:border-slate-300"
                    />
                    <span class="text-slate-400 text-xs font-bold">至</span>
                    <input
                      type="date"
                      v-model="searchParams.endDate"
                      @change="onCustomDateChange"
                      class="px-3 py-1 border border-slate-200 rounded-lg text-xs font-semibold focus:ring-2 focus:ring-hailong-primary/20 outline-none hover:border-slate-300"
                    />
                  </template>
                </div>
              </div>
            </div>
          </div>

          <!-- 检索数量统计 -->
          <div class="mb-4 flex justify-between items-center text-xs md:text-sm text-slate-500 font-medium">
            <div>
              共筛选出 <span class="text-hailong-primary font-bold">{{ total }}</span> 条招标公告
            </div>
          </div>

          <!-- 状态：加载中 -->
          <div v-if="loading" class="text-center py-32 bg-white rounded-2xl shadow-sm border border-slate-100">
            <div class="inline-block animate-spin rounded-full h-12 w-12 border-4 border-hailong-primary border-t-transparent"></div>
            <p class="mt-4 text-slate-400 text-sm">正在检索数据，请稍后...</p>
          </div>

          <!-- 状态：无结果 -->
          <div v-else-if="announcements.length === 0" class="text-center py-24 bg-white rounded-2xl shadow-[0_8px_30px_rgb(0,0,0,0.02)] border border-slate-100">
            <div class="w-16 h-16 bg-slate-50 text-slate-300 rounded-full flex items-center justify-center mx-auto mb-4">
              <svg class="w-8 h-8" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9.172 16.172a4 4 0 015.656 0M9 10h.01M15 10h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z" />
              </svg>
            </div>
            <p class="text-slate-500 font-bold mb-1">暂无符合筛选要求的公告</p>
            <p class="text-slate-400 text-xs">建议尝试缩短搜索词或重置筛选条件</p>
          </div>

          <!-- 数据列表 -->
          <div v-else class="space-y-4">
            <div
              v-for="announcement in announcements"
              :key="announcement.id"
              @click="handleViewDetail(announcement.id)"
              :class="[
                'group bg-white rounded-2xl p-6 shadow-[0_8px_30px_rgb(0,0,0,0.02)] hover:shadow-[0_15px_35px_rgba(40,120,255,0.06)] hover:-translate-y-0.5 border-l-4 border-y border-r border-slate-100/80 transition-all duration-300 cursor-pointer flex flex-col md:grid md:grid-cols-4 gap-5',
                announcement.businessType === 'GOV_PROCUREMENT' ? 'border-l-hailong-primary' :
                announcement.businessType === 'CONSTRUCTION' ? 'border-l-hailong-secondary' : 'border-l-slate-400'
              ]"
            >
              <!-- 左侧核心内容 (占比 3/4) -->
              <div class="md:col-span-3 flex flex-col justify-between space-y-4">
                <div>
                  <!-- 标签分类排版 -->
                  <div class="flex flex-wrap items-center gap-2 mb-3.5">
                    <span
                      :class="[
                        'px-2.5 py-0.5 rounded-md text-[10px] font-bold whitespace-nowrap shadow-sm text-white',
                        announcement.businessType === 'GOV_PROCUREMENT'
                          ? 'bg-gradient-to-r from-blue-500 to-blue-600'
                          : announcement.businessType === 'CONSTRUCTION'
                          ? 'bg-gradient-to-r from-orange-500 to-orange-600'
                          : 'bg-slate-500'
                      ]"
                    >
                      {{ announcement.businessType === 'GOV_PROCUREMENT' ? '政府采购' :
                         announcement.businessType === 'CONSTRUCTION' ? '建设工程' : '其他' }}
                    </span>
                    
                    <span
                      v-if="announcement.businessType === 'GOV_PROCUREMENT' && announcement.procurementType"
                      class="px-2.5 py-0.5 rounded-md text-[10px] font-semibold bg-slate-50 text-slate-600 border border-slate-100"
                    >
                      {{ announcement.procurementType === 'goods' ? '货物' :
                         announcement.procurementType === 'service' ? '服务' :
                         announcement.procurementType === 'project' ? '工程' : announcement.procurementType }}
                    </span>

                    <span
                      :class="[
                        'ml-auto px-2.5 py-0.5 rounded-md text-[10px] font-bold flex items-center gap-1 border',
                        announcement.noticeType === 'bidding' ? 'bg-blue-50 text-hailong-primary border-blue-100/30' :
                        announcement.noticeType === 'result' ? 'bg-emerald-50 text-emerald-600 border-emerald-100/30' :
                        announcement.noticeType === 'correction' ? 'bg-amber-50 text-amber-600 border-amber-100/30' :
                        'bg-slate-50 text-slate-600 border-slate-100'
                      ]"
                    >
                      <span class="w-1.5 h-1.5 rounded-full animate-pulse" :class="[
                        announcement.noticeType === 'bidding' ? 'bg-hailong-primary' :
                        announcement.noticeType === 'result' ? 'bg-emerald-500' :
                        announcement.noticeType === 'correction' ? 'bg-amber-500' :
                        'bg-slate-400'
                      ]"></span>
                      {{ getNoticeTypeLabel(announcement.noticeType) }}
                    </span>
                  </div>

                  <!-- 公告标题 -->
                  <h3 class="text-base md:text-lg font-bold text-slate-800 leading-snug group-hover:text-hailong-primary transition-colors line-clamp-2">
                    {{ announcement.title }}
                  </h3>
                </div>

                <!-- 商务辅助信息 -->
                <div class="flex flex-wrap gap-y-2 gap-x-5 text-xs text-slate-500 pt-3 border-t border-slate-100/60">
                  <div v-if="announcement.bidder" class="flex items-center gap-1.5 min-w-0">
                    <svg class="w-4 h-4 text-slate-400 flex-shrink-0" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                      <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 21V5a2 2 0 00-2-2H7a2 2 0 00-2 2v16m14 0h2m-2 0h-5m-9 0H3m2 0h5M9 7h1m-1 4h1m4-4h1m-1 4h1m-5 10v-5a1 1 0 011-1h2a1 1 0 011 1v5m-4 0h4" />
                    </svg>
                    <span class="truncate">招标人：<span class="font-bold text-slate-700">{{ announcement.bidder }}</span></span>
                  </div>
                  
                  <div v-if="announcement.projectRegion" class="flex items-center gap-1.5">
                    <svg class="w-4 h-4 text-slate-400 flex-shrink-0" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                      <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M17.657 16.657L13.414 20.9a1.998 1.998 0 01-2.827 0l-4.244-4.243a8 8 0 1111.314 0z" />
                      <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M15 11a3 3 0 11-6 0 3 3 0 016 0z" />
                    </svg>
                    <span>区域：<span class="font-bold text-slate-700">{{ announcement.projectRegion }}</span></span>
                  </div>

                  <div class="flex items-center gap-1.5 ml-auto">
                    <svg class="w-4 h-4 text-slate-400 flex-shrink-0" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                      <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M8 7V3m8 4V3m-9 8h10M5 21h14a2 2 0 002-2V7a2 2 0 00-2-2H5a2 2 0 00-2 2v12a2 2 0 002 2z" />
                    </svg>
                    <span>发布：<span class="text-slate-600 font-semibold">{{ formatDate(announcement.publishTime) }}</span></span>
                  </div>
                </div>
              </div>

              <!-- 右侧突出价值面板 (占比 1/4) -->
              <div class="md:col-span-1 border-t md:border-t-0 md:border-l border-slate-100 pt-4 md:pt-0 md:pl-5 flex flex-col justify-center items-end">
                <!-- 中标人高亮 -->
                <div v-if="announcement.winner" class="w-full flex flex-col items-end">
                  <span class="text-[10px] font-bold text-slate-400 tracking-wider mb-1 uppercase">中标单位</span>
                  <span class="px-2.5 py-1 bg-emerald-50/50 text-emerald-700 rounded-lg text-xs font-bold border border-emerald-100/50 truncate max-w-full text-right" :title="announcement.winner">
                    {{ announcement.winner }}
                  </span>
                </div>

                <!-- 预算金额高亮 -->
                <div v-else-if="announcement.budgetAmount" class="w-full flex flex-col items-end">
                  <span class="text-[10px] font-bold text-slate-400 tracking-wider mb-1.5 uppercase">采购预算</span>
                  <div class="text-hailong-primary font-tech flex items-baseline gap-0.5">
                    <span class="text-xs font-extrabold">¥</span>
                    <span class="text-2xl font-extrabold tracking-tight">{{ announcement.budgetAmount }}</span>
                    <span class="text-[10px] font-bold ml-0.5 text-slate-500">万</span>
                  </div>
                </div>

                <!-- 默认展示浏览数 -->
                <div v-else class="text-slate-400 flex items-center gap-1 text-xs">
                  <svg class="w-3.5 h-3.5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M15 12a3 3 0 11-6 0 3 3 0 016 0z" />
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M2.458 12C3.732 7.943 7.523 5 12 5c4.478 0 8.268 2.943 9.542 7-1.274 4.057-5.064 7-9.542 7-4.477 0-8.268-2.943-9.542-7z" />
                  </svg>
                  <span>{{ announcement.viewCount || 0 }} 次浏览</span>
                </div>

                <!-- 悬浮箭头 -->
                <div class="mt-4 flex items-center gap-1 text-xs font-bold text-hailong-primary transition-all group-hover:translate-x-1">
                  <span>查看详情</span>
                  <svg class="w-3.5 h-3.5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 5l7 7-7 7" />
                  </svg>
                </div>
              </div>
            </div>
          </div>

          <!-- 圆角精细分页器 -->
          <div v-if="total > 0" class="mt-12 flex items-center justify-center gap-2">
            <button
              @click="handlePageChange(currentPage - 1)"
              :disabled="currentPage === 1"
              :class="[
                'px-4 py-2 rounded-xl border text-sm font-semibold transition-all duration-200',
                currentPage === 1
                  ? 'border-slate-100 text-slate-300 cursor-not-allowed bg-slate-50/50'
                  : 'border-slate-200 text-slate-600 bg-white hover:bg-slate-50 hover:border-slate-300'
              ]"
            >
              上一页
            </button>

            <div class="flex gap-1.5">
              <button
                v-for="page in displayPages"
                :key="page"
                @click="page !== '...' && handlePageChange(page)"
                :class="[
                  'w-10 h-10 rounded-xl text-sm font-bold transition-all duration-200 flex items-center justify-center',
                  page === currentPage
                    ? 'bg-gradient-to-r from-hailong-primary to-hailong-secondary text-white shadow-md shadow-blue-500/15'
                    : page === '...'
                    ? 'text-slate-400 cursor-default'
                    : 'text-slate-600 bg-white border border-slate-200 hover:bg-slate-50 hover:border-slate-300'
                ]"
              >
                {{ page }}
              </button>
            </div>

            <button
              @click="handlePageChange(currentPage + 1)"
              :disabled="currentPage === totalPages"
              :class="[
                'px-4 py-2 rounded-xl border text-sm font-semibold transition-all duration-200',
                currentPage === totalPages
                  ? 'border-slate-100 text-slate-300 cursor-not-allowed bg-slate-50/50'
                  : 'border-slate-200 text-slate-600 bg-white hover:bg-slate-50 hover:border-slate-300'
              ]"
            >
              下一页
            </button>
          </div>
        </div>
      </div>
    </div>

    <Footer />
  </div>
</template>

<script setup>
import { ref, computed, onMounted, watch } from 'vue'
import { useRouter, useRoute } from 'vue-router'
import Header from '@/components/Header.vue'
import Footer from '@/components/Footer.vue'
import { getAnnouncementList } from '@/api/announcement.js'
import { getProvinceList, getCityList, getDistrictList } from '@/api/region.js'
import { formatDate } from '@/utils/date'

const router = useRouter()
const route = useRoute()

// 业务类型选项
const businessTypes = [
  { label: '全部业务', value: '' },
  { label: '政府采购', value: 'GOV_PROCUREMENT' },
  { label: '建设工程', value: 'CONSTRUCTION' }
]

// 公告类型选项 - 全部业务类型
const announcementTypesAll = [
  { label: '全部公告', value: '' },
  { label: '招标采购公告', value: 'bidding' },
  { label: '更正答疑公告', value: 'correction' },
  { label: '中标结果公示', value: 'result' }
]

// 公告类型选项 - 政府采购
const announcementTypesGov = [
  { label: '全部采购', value: '' },
  { label: '招标公告', value: 'bidding' },
  { label: '更正公告', value: 'correction' },
  { label: '结果公告', value: 'result' }
]

// 公告类型选项 - 建设工程
const announcementTypesConstruction = [
  { label: '全部工程', value: '' },
  { label: '采购公告', value: 'bidding' },
  { label: '更正公告', value: 'correction' },
  { label: '结果公告', value: 'result' }
]

// 采购类型选项（仅政府采购时显示）
const procurementTypes = [
  { label: '全部类别', value: '' },
  { label: '货物', value: 'goods' },
  { label: '服务', value: 'service' },
  { label: '工程', value: 'project' }
]

// 根据业务类型动态获取公告类型选项
const currentAnnouncementTypes = computed(() => {
  if (searchParams.value.businessType === 'GOV_PROCUREMENT') {
    return announcementTypesGov
  } else if (searchParams.value.businessType === 'CONSTRUCTION') {
    return announcementTypesConstruction
  } else {
    return announcementTypesAll
  }
})

// 省市区数据
const provinces = ref([])
const cities = ref([])
const districts = ref([])
const selectedProvinceCode = ref('')
const selectedCityCode = ref('')

// 时间范围选项
const timeRanges = [
  { label: '全部时间', value: '' },
  { label: '当天', value: 'today' },
  { label: '近三天', value: '3days' },
  { label: '近一周', value: 'week' },
  { label: '近一月', value: 'month' }
]

// 搜索参数
const searchParams = ref({
  businessType: '',
  noticeType: '',
  procurementType: '',
  province: '',
  city: '',
  district: '',
  timeRange: '',
  startDate: '',
  endDate: '',
  keyword: ''
})

// 其他状态
const showCustomDatePicker = ref(false)
const loading = ref(false)
const announcements = ref([])
const total = ref(0)
const currentPage = ref(1)
const pageSize = ref(10)

// 加载省份列表
const loadProvinces = async () => {
  try {
    const res = await getProvinceList()
    if (res.success && res.data) {
      provinces.value = res.data
    }
  } catch (error) {
    console.error('加载省份列表失败:', error)
  }
}

// 加载城市列表
const loadCities = async (provinceCode) => {
  if (!provinceCode) {
    cities.value = []
    return
  }
  try {
    const res = await getCityList(provinceCode)
    if (res.success && res.data) {
      cities.value = res.data
    }
  } catch (error) {
    console.error('加载城市列表失败:', error)
  }
}

// 加载区县列表
const loadDistricts = async (cityCode) => {
  if (!cityCode) {
    districts.value = []
    return
  }
  try {
    const res = await getDistrictList(cityCode)
    if (res.success && res.data) {
      districts.value = res.data
    }
  } catch (error) {
    console.error('加载区县列表失败:', error)
  }
}

// 省份变化处理
const onProvinceChange = async () => {
  searchParams.value.city = ''
  searchParams.value.district = ''
  cities.value = []
  districts.value = []
  
  if (searchParams.value.province) {
    const province = provinces.value.find(p => p.regionName === searchParams.value.province)
    if (province) {
      selectedProvinceCode.value = province.regionCode
      await loadCities(province.regionCode)
    }
  } else {
    selectedProvinceCode.value = ''
  }
  currentPage.value = 1
  updateUrlQuery()
}

// 城市变化处理
const onCityChange = async () => {
  searchParams.value.district = ''
  districts.value = []
  
  if (searchParams.value.city) {
    const city = cities.value.find(c => c.regionName === searchParams.value.city)
    if (city) {
      selectedCityCode.value = city.regionCode
      await loadDistricts(city.regionCode)
    }
  } else {
    selectedCityCode.value = ''
  }
  currentPage.value = 1
  updateUrlQuery()
}

// 区县变化处理
const onDistrictChange = () => {
  currentPage.value = 1
  updateUrlQuery()
}

// 清除关键字
const clearKeyword = () => {
  searchParams.value.keyword = ''
  handleSearch()
}

// 绑定公告类型切换事件
const handleNoticeTypeChange = (value) => {
  searchParams.value.noticeType = value
  currentPage.value = 1
  updateUrlQuery()
}

// 绑定采购类型切换事件
const handleProcurementTypeChange = (value) => {
  searchParams.value.procurementType = value
  currentPage.value = 1
  updateUrlQuery()
}

// 动态同步筛选条件至 URL Query
const updateUrlQuery = () => {
  const query = {}
  if (searchParams.value.businessType) query.tab = searchParams.value.businessType
  if (searchParams.value.noticeType) query.noticeType = searchParams.value.noticeType
  if (searchParams.value.procurementType) query.procurementType = searchParams.value.procurementType
  if (searchParams.value.keyword) query.keyword = searchParams.value.keyword
  if (searchParams.value.province) query.province = searchParams.value.province
  if (searchParams.value.city) query.city = searchParams.value.city
  if (searchParams.value.district) query.district = searchParams.value.district
  if (searchParams.value.timeRange) query.timeRange = searchParams.value.timeRange
  if (currentPage.value > 1) query.page = currentPage.value

  router.replace({ query }).catch(err => {
    if (err.name !== 'NavigationDuplicated') console.error(err)
  })
}

// 业务类型变化处理
const handleBusinessTypeChange = (value) => {
  searchParams.value.businessType = value
  if (value !== 'GOV_PROCUREMENT') {
    searchParams.value.procurementType = ''
  }
  currentPage.value = 1
  updateUrlQuery()
}

// 时间范围选择
const selectTimeRange = (value) => {
  searchParams.value.timeRange = value
  showCustomDatePicker.value = false
  
  const today = new Date()
  let startDate = ''
  
  switch (value) {
    case 'today':
      startDate = today.toISOString().split('T')[0]
      searchParams.value.startDate = startDate
      searchParams.value.endDate = startDate
      break
    case '3days':
      startDate = new Date(today.getTime() - 3 * 24 * 60 * 60 * 1000).toISOString().split('T')[0]
      searchParams.value.startDate = startDate
      searchParams.value.endDate = today.toISOString().split('T')[0]
      break
    case 'week':
      startDate = new Date(today.getTime() - 7 * 24 * 60 * 60 * 1000).toISOString().split('T')[0]
      searchParams.value.startDate = startDate
      searchParams.value.endDate = today.toISOString().split('T')[0]
      break
    case 'month':
      startDate = new Date(today.getTime() - 30 * 24 * 60 * 60 * 1000).toISOString().split('T')[0]
      searchParams.value.startDate = startDate
      searchParams.value.endDate = today.toISOString().split('T')[0]
      break
    default:
      searchParams.value.startDate = ''
      searchParams.value.endDate = ''
  }
  currentPage.value = 1
  updateUrlQuery()
}

// 自定义日期变化
const onCustomDateChange = () => {
  searchParams.value.timeRange = 'custom'
  currentPage.value = 1
  updateUrlQuery()
}

// 加载公告列表
const loadAnnouncements = async () => {
  loading.value = true
  try {
    const params = {
      pageNumber: currentPage.value,
      pageSize: pageSize.value
    }
    
    if (searchParams.value.businessType) {
      params.businessType = searchParams.value.businessType
    }
    if (searchParams.value.noticeType) {
      params.noticeType = searchParams.value.noticeType
    }
    if (searchParams.value.businessType === 'GOV_PROCUREMENT' && searchParams.value.procurementType) {
      params.procurementType = searchParams.value.procurementType
    }
    if (searchParams.value.province) {
      params.province = searchParams.value.province
    }
    if (searchParams.value.city) {
      params.city = searchParams.value.city
    }
    if (searchParams.value.district) {
      params.district = searchParams.value.district
    }
    if (searchParams.value.keyword) {
      params.keyword = searchParams.value.keyword
    }
    if (searchParams.value.startDate) {
      params.startDate = searchParams.value.startDate
    }
    if (searchParams.value.endDate) {
      params.endDate = searchParams.value.endDate
    }
    
    const res = await getAnnouncementList(params)
    if (res.success && res.data) {
      announcements.value = res.data.items || []
      total.value = res.data.totalCount || 0
    }
  } catch (error) {
    console.error('加载公告列表失败:', error)
  } finally {
    loading.value = false
  }
}

// 搜索
const handleSearch = () => {
  currentPage.value = 1
  updateUrlQuery()
}

// 重置
const handleReset = () => {
  searchParams.value = {
    businessType: '',
    noticeType: '',
    procurementType: '',
    province: '',
    city: '',
    district: '',
    timeRange: '',
    startDate: '',
    endDate: '',
    keyword: ''
  }
  showCustomDatePicker.value = false
  cities.value = []
  districts.value = []
  selectedProvinceCode.value = ''
  selectedCityCode.value = ''
  currentPage.value = 1
  updateUrlQuery()
}

// 查看详情
const handleViewDetail = (id) => {
  router.push(`/detail/announcement/${id}`)
}

// 分页变化
const handlePageChange = (page) => {
  currentPage.value = page
  updateUrlQuery()
  window.scrollTo({ top: 0, behavior: 'smooth' })
}

// 监听路由 Query 变化并重新加载数据
watch(
  () => route.query,
  async (newQuery) => {
    // 仅在当前处于公告列表页路由时才响应变更（规避离开路由时的干扰）
    if (route.name !== 'Announcements') return

    searchParams.value.businessType = newQuery.tab || ''
    searchParams.value.noticeType = newQuery.noticeType || ''
    searchParams.value.procurementType = newQuery.procurementType || ''
    searchParams.value.keyword = newQuery.keyword || ''
    searchParams.value.province = newQuery.province || ''
    searchParams.value.city = newQuery.city || ''
    searchParams.value.district = newQuery.district || ''
    searchParams.value.timeRange = newQuery.timeRange || ''
    
    // 如果存在省市区名称，反推 region code 并加载联动子菜单数据
    if (searchParams.value.province && provinces.value.length > 0) {
      const province = provinces.value.find(p => p.regionName === searchParams.value.province)
      if (province) {
        selectedProvinceCode.value = province.regionCode
        await loadCities(province.regionCode)
        if (searchParams.value.city) {
          const city = cities.value.find(c => c.regionName === searchParams.value.city)
          if (city) {
            selectedCityCode.value = city.regionCode
            await loadDistricts(city.regionCode)
          }
        }
      }
    } else {
      selectedProvinceCode.value = ''
      selectedCityCode.value = ''
      cities.value = []
      districts.value = []
    }
    
    currentPage.value = parseInt(newQuery.page) || 1
    await loadAnnouncements()
  },
  { deep: true }
)

// 离开路由前将 query 暂存至 sessionStorage，用于无参数返回时进行自动恢复
import { onBeforeRouteLeave } from 'vue-router'
onBeforeRouteLeave((to, from) => {
  sessionStorage.setItem('announcements_query', JSON.stringify(route.query))
})

// 初始化挂载
onMounted(async () => {
  await loadProvinces()
  
  let query = route.query
  // 若 URL query 为空，尝试从 sessionStorage 恢复缓存的条件
  if (Object.keys(query).length === 0) {
    const savedQueryStr = sessionStorage.getItem('announcements_query')
    if (savedQueryStr) {
      try {
        const savedQuery = JSON.parse(savedQueryStr)
        if (Object.keys(savedQuery).length > 0) {
          router.replace({ query: savedQuery })
          return // 让 watch(route.query) 去接管数据加载流程
        }
      } catch (e) {
        console.error('解析缓存筛选条件失败:', e)
      }
    }
  }

  // 若无缓存或直接进入，手动初始化字段
  searchParams.value.businessType = query.tab || ''
  searchParams.value.noticeType = query.noticeType || ''
  searchParams.value.procurementType = query.procurementType || ''
  searchParams.value.keyword = query.keyword || ''
  searchParams.value.province = query.province || ''
  searchParams.value.city = query.city || ''
  searchParams.value.district = query.district || ''
  searchParams.value.timeRange = query.timeRange || ''
  
  if (searchParams.value.province && provinces.value.length > 0) {
    const province = provinces.value.find(p => p.regionName === searchParams.value.province)
    if (province) {
      selectedProvinceCode.value = province.regionCode
      await loadCities(province.regionCode)
      if (searchParams.value.city) {
        const city = cities.value.find(c => c.regionName === searchParams.value.city)
        if (city) {
          selectedCityCode.value = city.regionCode
          await loadDistricts(city.regionCode)
        }
      }
    }
  }
  
  currentPage.value = parseInt(query.page) || 1
  await loadAnnouncements()
})
</script>

<style scoped>
.container-wide {
  max-width: 1400px;
  margin: 0 auto;
  padding: 0 1.5rem;
}

.line-clamp-2 {
  display: -webkit-box;
  -webkit-line-clamp: 2;
  -webkit-box-orient: vertical;
  overflow: hidden;
}

.bg-grid-pattern {
  background-image: linear-gradient(to right, rgba(255,255,255,0.05) 1px, transparent 1px),
                    linear-gradient(to bottom, rgba(255,255,255,0.05) 1px, transparent 1px);
  background-size: 24px 24px;
}
</style>
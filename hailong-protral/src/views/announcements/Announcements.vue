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
        <p class="text-base md:text-lg text-slate-300 font-medium max-w-2xl mx-auto">
          招标采购信息
        </p>
      </div>
    </div>

    <!-- 内容区域 - 列表 -->
    <div v-if="!selectedHashId" class="py-16 bg-white">
      <div class="container-wide">
        <div class="animate-fade-in">
      <!-- 搜索筛选区域 -->
      <div class="bg-white rounded-xl shadow-sm p-6 mb-6">
        <!-- 关键字搜索 - 主搜索框 -->
        <div class="mb-5">
          <div class="flex gap-3">
            <div class="relative flex-1">
              <svg class="absolute left-4 top-1/2 transform -translate-y-1/2 w-5 h-5 text-gray-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M21 21l-6-6m2-5a7 7 0 11-14 0 7 7 0 0114 0z" />
              </svg>
              <input
                v-model="searchParams.keyword"
                type="text"
                placeholder="请输入项目名称、招标单位等关键字"
                class="w-full pl-12 pr-4 py-3 border border-gray-300 rounded-lg focus:ring-2 focus:ring-hailong-primary focus:border-transparent outline-none transition-all"
                @keyup.enter="handleSearch"
              />
            </div>
            <button
              @click="handleSearch"
              class="px-8 py-3 bg-gradient-to-r from-hailong-primary to-hailong-secondary text-white rounded-lg hover:shadow-lg transition-all font-medium"
            >
              搜索
            </button>
            <button
              @click="handleReset"
              class="px-6 py-3 bg-gray-100 text-gray-700 rounded-lg hover:bg-gray-200 transition-all font-medium"
            >
              重置
            </button>
          </div>
        </div>

        <!-- 筛选条件 -->
        <div class="space-y-3 pt-3 border-t border-gray-100">
          <!-- 业务类型和采购类型 -->
          <div class="flex items-center gap-4 flex-wrap">
            <label class="text-sm font-medium text-gray-700 whitespace-nowrap w-16">业务类型</label>
            <button
              v-for="type in businessTypes"
              :key="type.value"
              @click="handleBusinessTypeChange(type.value)"
              :class="[
                'px-5 py-1.5 rounded-lg text-sm font-medium transition-all',
                searchParams.businessType === type.value
                  ? 'bg-gradient-to-r from-hailong-primary to-hailong-secondary text-white shadow-lg'
                  : 'bg-gray-100 text-gray-700 hover:bg-gray-200'
              ]"
            >
              {{ type.label }}
            </button>

            <!-- 采购类型 - 仅在选择政府采购时显示 -->
            <template v-if="searchParams.businessType === 'GOV_PROCUREMENT'">
              <label class="text-sm font-medium text-gray-700 whitespace-nowrap w-16 ml-6">采购类型</label>
              <button
                v-for="type in procurementTypes"
                :key="type.value"
                @click="searchParams.procurementType = type.value"
                :class="[
                  'px-5 py-1.5 rounded-lg text-sm font-medium transition-all',
                  searchParams.procurementType === type.value
                    ? 'bg-gradient-to-r from-hailong-primary to-hailong-secondary text-white shadow-lg'
                    : 'bg-gray-100 text-gray-700 hover:bg-gray-200'
                ]"
              >
                {{ type.label }}
              </button>
            </template>
          </div>

          <!-- 公告类型 -->
          <div class="flex items-center gap-4">
            <label class="text-sm font-medium text-gray-700 whitespace-nowrap w-16">公告类型</label>
            <div class="flex flex-wrap gap-2">
              <button
                v-for="type in currentAnnouncementTypes"
                :key="type.value"
                @click="searchParams.noticeType = type.value"
                :class="[
                  'px-5 py-1.5 rounded-lg text-sm font-medium transition-all',
                  searchParams.noticeType === type.value
                    ? 'bg-gradient-to-r from-hailong-primary to-hailong-secondary text-white shadow-lg'
                    : 'bg-gray-100 text-gray-700 hover:bg-gray-200'
                ]"
              >
                {{ type.label }}
              </button>
            </div>
          </div>

          <!-- 项目区域和发布时间 -->
          <div class="flex items-center gap-4 flex-wrap">
            <div class="flex items-center gap-4">
              <label class="text-sm font-medium text-gray-700 whitespace-nowrap w-16">项目区域</label>
              
              <!-- 省份选择 -->
              <div class="relative">
                <svg class="absolute left-3 top-1/2 transform -translate-y-1/2 w-4 h-4 text-gray-400 pointer-events-none" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M17.657 16.657L13.414 20.9a1.998 1.998 0 01-2.827 0l-4.244-4.243a8 8 0 1111.314 0z" />
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M15 11a3 3 0 11-6 0 3 3 0 016 0z" />
                </svg>
                <select
                  v-model="searchParams.province"
                  @change="onProvinceChange"
                  class="pl-9 pr-8 py-1.5 border border-gray-300 rounded-lg focus:ring-2 focus:ring-hailong-primary focus:border-transparent outline-none transition-all text-sm bg-white hover:border-hailong-primary cursor-pointer appearance-none"
                  style="min-width: 120px;"
                >
                  <option value="">全部省份</option>
                  <option v-for="province in provinces" :key="province.regionCode" :value="province.regionName">
                    {{ province.regionName }}
                  </option>
                </select>
                <svg class="absolute right-3 top-1/2 transform -translate-y-1/2 w-4 h-4 text-gray-400 pointer-events-none" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 9l-7 7-7-7" />
                </svg>
              </div>

              <!-- 城市选择 -->
              <div class="relative" v-if="searchParams.province">
                <svg class="absolute left-3 top-1/2 transform -translate-y-1/2 w-4 h-4 text-gray-400 pointer-events-none" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 21V5a2 2 0 00-2-2H7a2 2 0 00-2 2v16m14 0h2m-2 0h-5m-9 0H3m2 0h5M9 7h1m-1 4h1m4-4h1m-1 4h1m-5 10v-5a1 1 0 011-1h2a1 1 0 011 1v5m-4 0h4" />
                </svg>
                <select
                  v-model="searchParams.city"
                  @change="onCityChange"
                  class="pl-9 pr-8 py-1.5 border border-gray-300 rounded-lg focus:ring-2 focus:ring-hailong-primary focus:border-transparent outline-none transition-all text-sm bg-white hover:border-hailong-primary cursor-pointer appearance-none"
                  style="min-width: 120px;"
                >
                  <option value="">全部城市</option>
                  <option v-for="city in cities" :key="city.regionCode" :value="city.regionName">
                    {{ city.regionName }}
                  </option>
                </select>
                <svg class="absolute right-3 top-1/2 transform -translate-y-1/2 w-4 h-4 text-gray-400 pointer-events-none" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 9l-7 7-7-7" />
                </svg>
              </div>

              <!-- 区县选择 -->
              <div class="relative" v-if="searchParams.city">
                <svg class="absolute left-3 top-1/2 transform -translate-y-1/2 w-4 h-4 text-gray-400 pointer-events-none" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M3 12l2-2m0 0l7-7 7 7M5 10v10a1 1 0 001 1h3m10-11l2 2m-2-2v10a1 1 0 01-1 1h-3m-6 0a1 1 0 001-1v-4a1 1 0 011-1h2a1 1 0 011 1v4a1 1 0 001 1m-6 0h6" />
                </svg>
                <select
                  v-model="searchParams.district"
                  class="pl-9 pr-8 py-1.5 border border-gray-300 rounded-lg focus:ring-2 focus:ring-hailong-primary focus:border-transparent outline-none transition-all text-sm bg-white hover:border-hailong-primary cursor-pointer appearance-none"
                  style="min-width: 120px;"
                >
                  <option value="">全部区县</option>
                  <option v-for="district in districts" :key="district.regionCode" :value="district.regionName">
                    {{ district.regionName }}
                  </option>
                </select>
                <svg class="absolute right-3 top-1/2 transform -translate-y-1/2 w-4 h-4 text-gray-400 pointer-events-none" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 9l-7 7-7-7" />
                </svg>
              </div>
            </div>

            <label class="text-sm font-medium text-gray-700 whitespace-nowrap w-16">发布时间</label>
            <button
              v-for="time in timeRanges"
              :key="time.value"
              @click="selectTimeRange(time.value)"
              :class="[
                'px-4 py-1.5 rounded-lg text-sm font-medium transition-all',
                searchParams.timeRange === time.value
                  ? 'bg-gradient-to-r from-hailong-primary to-hailong-secondary text-white shadow-lg'
                  : 'bg-gray-100 text-gray-700 hover:bg-gray-200'
              ]"
            >
              {{ time.label }}
            </button>
            <button
              @click="showCustomDatePicker = !showCustomDatePicker"
              :class="[
                'px-4 py-1.5 rounded-lg text-sm font-medium transition-all flex items-center gap-1',
                searchParams.timeRange === 'custom'
                  ? 'bg-gradient-to-r from-hailong-primary to-hailong-secondary text-white shadow-lg'
                  : 'bg-gray-100 text-gray-700 hover:bg-gray-200'
              ]"
            >
              <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M8 7V3m8 4V3m-9 8h10M5 21h14a2 2 0 002-2V7a2 2 0 00-2-2H5a2 2 0 00-2 2v12a2 2 0 002 2z" />
              </svg>
              自定义
            </button>

            <!-- 自定义日期选择器 -->
            <template v-if="showCustomDatePicker">
              <div class="relative">
                <svg class="absolute left-3 top-1/2 transform -translate-y-1/2 w-4 h-4 text-gray-400 pointer-events-none" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M8 7V3m8 4V3m-9 8h10M5 21h14a2 2 0 002-2V7a2 2 0 00-2-2H5a2 2 0 00-2 2v12a2 2 0 002 2z" />
                </svg>
                <input
                  type="date"
                  v-model="searchParams.startDate"
                  @change="onCustomDateChange"
                  class="pl-9 pr-3 py-1.5 border border-gray-300 rounded-lg focus:ring-2 focus:ring-hailong-primary focus:border-transparent outline-none transition-all text-sm hover:border-hailong-primary"
                />
              </div>
              <span class="text-gray-500 text-sm font-medium">至</span>
              <div class="relative">
                <svg class="absolute left-3 top-1/2 transform -translate-y-1/2 w-4 h-4 text-gray-400 pointer-events-none" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M8 7V3m8 4V3m-9 8h10M5 21h14a2 2 0 002-2V7a2 2 0 00-2-2H5a2 2 0 00-2 2v12a2 2 0 002 2z" />
                </svg>
                <input
                  type="date"
                  v-model="searchParams.endDate"
                  @change="onCustomDateChange"
                  class="pl-9 pr-3 py-1.5 border border-gray-300 rounded-lg focus:ring-2 focus:ring-hailong-primary focus:border-transparent outline-none transition-all text-sm hover:border-hailong-primary"
                />
              </div>
            </template>
          </div>
        </div>
      </div>

      <!-- 结果统计 -->
      <div class="mb-4 text-sm text-gray-600">
        共找到 <span class="text-hailong-primary font-semibold">{{ total }}</span> 条公告
      </div>

      <!-- 公告列表 -->
      <div v-if="loading" class="text-center py-20">
        <div class="inline-block animate-spin rounded-full h-12 w-12 border-4 border-hailong-primary border-t-transparent"></div>
        <p class="mt-4 text-gray-500">加载中...</p>
      </div>

      <div v-else-if="announcements.length === 0" class="text-center py-20">
        <svg class="w-20 h-20 mx-auto text-gray-300 mb-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
          <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 12h6m-6 4h6m2 5H7a2 2 0 01-2-2V5a2 2 0 012-2h5.586a1 1 0 01.707.293l5.414 5.414a1 1 0 01.293.707V19a2 2 0 01-2 2z" />
        </svg>
        <p class="text-gray-500">暂无公告信息</p>
      </div>

      <div v-else class="space-y-4">
        <div
          v-for="announcement in announcements"
          :key="announcement.id"
          @click="handleViewDetail(announcement.hashId || announcement.id)"
          :class="[
            'bg-white rounded-xl p-6 shadow-sm hover:shadow-lg transition-all cursor-pointer border-l-4',
            announcement.businessType === 'GOV_PROCUREMENT' ? 'border-hailong-primary' :
            announcement.businessType === 'CONSTRUCTION' ? 'border-hailong-secondary' : 'border-hailong-primary'
          ]"
        >
          <!-- 业务类型和采购类型标签 -->
          <div class="flex items-center gap-2 mb-3">
            <span
              :class="[
                'px-3 py-1 rounded-lg text-xs font-bold whitespace-nowrap',
                announcement.businessType === 'GOV_PROCUREMENT'
                  ? 'bg-gradient-to-r from-blue-500 to-blue-600 text-white shadow-md'
                  : announcement.businessType === 'CONSTRUCTION'
                  ? 'bg-gradient-to-r from-orange-500 to-orange-600 text-white shadow-md'
                  : 'bg-gray-500 text-white'
              ]"
            >
              {{ announcement.businessType === 'GOV_PROCUREMENT' ? '政府采购' :
                 announcement.businessType === 'CONSTRUCTION' ? '建设工程' : '其他' }}
            </span>
            
            <!-- 采购类型 - 仅政府采购显示 -->
            <span
              v-if="announcement.businessType === 'GOV_PROCUREMENT' && announcement.procurementType"
              :class="[
                'px-3 py-1 rounded-lg text-xs font-semibold whitespace-nowrap',
                'bg-blue-50 text-blue-700 border border-blue-200'
              ]"
            >
              {{ announcement.procurementType === 'goods' ? '货物' :
                 announcement.procurementType === 'service' ? '服务' :
                 announcement.procurementType === 'project' ? '工程' : announcement.procurementType }}
            </span>

            <!-- 公告类型 - 优化样式 -->
            <span
              :class="[
                'ml-auto px-4 py-1.5 rounded-lg text-xs font-bold whitespace-nowrap flex items-center gap-1.5 shadow-sm',
                announcement.noticeType === 'bidding'
                  ? 'bg-gradient-to-r from-blue-500 to-indigo-600 text-white'
                  : announcement.noticeType === 'result'
                  ? 'bg-gradient-to-r from-green-500 to-emerald-600 text-white'
                  : announcement.noticeType === 'correction'
                  ? 'bg-gradient-to-r from-orange-500 to-amber-600 text-white'
                  : 'bg-gradient-to-r from-gray-500 to-gray-600 text-white'
              ]"
            >
              <svg v-if="announcement.noticeType === 'bidding'" class="w-4 h-4" fill="currentColor" viewBox="0 0 20 20">
                <path d="M9 2a1 1 0 000 2h2a1 1 0 100-2H9z"/>
                <path fill-rule="evenodd" d="M4 5a2 2 0 012-2 3 3 0 003 3h2a3 3 0 003-3 2 2 0 012 2v11a2 2 0 01-2 2H6a2 2 0 01-2-2V5zm3 4a1 1 0 000 2h.01a1 1 0 100-2H7zm3 0a1 1 0 000 2h3a1 1 0 100-2h-3zm-3 4a1 1 0 100 2h.01a1 1 0 100-2H7zm3 0a1 1 0 100 2h3a1 1 0 100-2h-3z" clip-rule="evenodd"/>
              </svg>
              <svg v-else-if="announcement.noticeType === 'result'" class="w-4 h-4" fill="currentColor" viewBox="0 0 20 20">
                <path fill-rule="evenodd" d="M6.267 3.455a3.066 3.066 0 001.745-.723 3.066 3.066 0 013.976 0 3.066 3.066 0 001.745.723 3.066 3.066 0 012.812 2.812c.051.643.304 1.254.723 1.745a3.066 3.066 0 010 3.976 3.066 3.066 0 00-.723 1.745 3.066 3.066 0 01-2.812 2.812 3.066 3.066 0 00-1.745.723 3.066 3.066 0 01-3.976 0 3.066 3.066 0 00-1.745-.723 3.066 3.066 0 01-2.812-2.812 3.066 3.066 0 00-.723-1.745 3.066 3.066 0 010-3.976 3.066 3.066 0 00.723-1.745 3.066 3.066 0 012.812-2.812zm7.44 5.252a1 1 0 00-1.414-1.414L9 10.586 7.707 9.293a1 1 0 00-1.414 1.414l2 2a1 1 0 001.414 0l4-4z" clip-rule="evenodd"/>
              </svg>
              <svg v-else-if="announcement.noticeType === 'correction'" class="w-4 h-4" fill="currentColor" viewBox="0 0 20 20">
                <path fill-rule="evenodd" d="M18 10a8 8 0 11-16 0 8 8 0 0116 0zm-7 4a1 1 0 11-2 0 1 1 0 012 0zm-1-9a1 1 0 00-1 1v4a1 1 0 102 0V6a1 1 0 00-1-1z" clip-rule="evenodd"/>
              </svg>
              <svg v-else class="w-4 h-4" fill="currentColor" viewBox="0 0 20 20">
                <path fill-rule="evenodd" d="M18 10a8 8 0 11-16 0 8 8 0 0116 0zm-7-4a1 1 0 11-2 0 1 1 0 012 0zM9 9a1 1 0 000 2v3a1 1 0 001 1h1a1 1 0 100-2v-3a1 1 0 00-1-1H9z" clip-rule="evenodd"/>
              </svg>
              {{ announcement.noticeTypeName }}
            </span>
          </div>

          <!-- 标题 -->
          <h3 class="text-lg font-bold text-gray-900 mb-4 hover:text-hailong-primary transition-colors line-clamp-2">
            {{ announcement.title }}
          </h3>

          <!-- 中标人信息 - 显眼位置 -->
          <div v-if="announcement.winner" class="mb-4 p-3 bg-gradient-to-r from-green-50 to-emerald-50 border-l-4 border-green-500 rounded-lg">
            <div class="flex items-center gap-2">
              <svg class="w-5 h-5 text-green-600" fill="currentColor" viewBox="0 0 20 20">
                <path fill-rule="evenodd" d="M6.267 3.455a3.066 3.066 0 001.745-.723 3.066 3.066 0 013.976 0 3.066 3.066 0 001.745.723 3.066 3.066 0 012.812 2.812c.051.643.304 1.254.723 1.745a3.066 3.066 0 010 3.976 3.066 3.066 0 00-.723 1.745 3.066 3.066 0 01-2.812 2.812 3.066 3.066 0 00-1.745.723 3.066 3.066 0 01-3.976 0 3.066 3.066 0 00-1.745-.723 3.066 3.066 0 01-2.812-2.812 3.066 3.066 0 00-.723-1.745 3.066 3.066 0 010-3.976 3.066 3.066 0 00.723-1.745 3.066 3.066 0 012.812-2.812zm7.44 5.252a1 1 0 00-1.414-1.414L9 10.586 7.707 9.293a1 1 0 00-1.414 1.414l2 2a1 1 0 001.414 0l4-4z" clip-rule="evenodd"/>
              </svg>
              <span class="text-sm text-gray-600 font-medium">中标人：</span>
              <span class="text-base text-gray-900 font-bold">{{ announcement.winner }}</span>
            </div>
          </div>

          <!-- 其他详细信息 -->
          <div class="grid grid-cols-1 md:grid-cols-3 gap-3 text-sm">
            <div v-if="announcement.bidder" class="flex items-center gap-2">
              <svg class="w-4 h-4 text-gray-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 21V5a2 2 0 00-2-2H7a2 2 0 00-2 2v16m14 0h2m-2 0h-5m-9 0H3m2 0h5M9 7h1m-1 4h1m4-4h1m-1 4h1m-5 10v-5a1 1 0 011-1h2a1 1 0 011 1v5m-4 0h4" />
              </svg>
              <span class="text-gray-500">招标人：</span>
              <span class="text-gray-700 font-medium truncate">{{ announcement.bidder }}</span>
            </div>
            <div v-if="announcement.projectRegion" class="flex items-center gap-2">
              <svg class="w-4 h-4 text-gray-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M17.657 16.657L13.414 20.9a1.998 1.998 0 01-2.827 0l-4.244-4.243a8 8 0 1111.314 0z" />
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M15 11a3 3 0 11-6 0 3 3 0 016 0z" />
              </svg>
              <span class="text-gray-500">项目区域：</span>
              <span class="text-gray-700 font-medium">{{ announcement.projectRegion }}</span>
            </div>
            <div v-if="announcement.publishTime" class="flex items-center gap-2">
              <svg class="w-4 h-4 text-gray-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M8 7V3m8 4V3m-9 8h10M5 21h14a2 2 0 002-2V7a2 2 0 00-2-2H5a2 2 0 00-2 2v12a2 2 0 002 2z" />
              </svg>
              <span class="text-gray-500">发布时间：</span>
              <span class="text-gray-700 font-medium">{{ formatDate(announcement.publishTime) }}</span>
            </div>
          </div>

          <!-- 底部信息 -->
          <div class="flex items-center justify-between mt-4 pt-4 border-t border-gray-100">
            <div class="flex items-center gap-4 text-xs text-gray-500">
              <span class="flex items-center gap-1">
                <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M15 12a3 3 0 11-6 0 3 3 0 016 0z" />
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M2.458 12C3.732 7.943 7.523 5 12 5c4.478 0 8.268 2.943 9.542 7-1.274 4.057-5.064 7-9.542 7-4.477 0-8.268-2.943-9.542-7z" />
                </svg>
                {{ announcement.viewCount || 0 }} 次浏览
              </span>
            </div>
            <span class="text-hailong-primary text-sm font-medium hover:underline">
              查看详情 →
            </span>
          </div>
        </div>
      </div>

      <!-- 分页组件 -->
      <div v-if="total > 0" class="mt-8 flex items-center justify-center gap-2">
        <button
          @click="handlePageChange(currentPage - 1)"
          :disabled="currentPage === 1"
          :class="[
            'px-4 py-2 rounded-lg border transition-all',
            currentPage === 1
              ? 'border-gray-200 text-gray-400 cursor-not-allowed'
              : 'border-gray-300 text-gray-700 hover:bg-gray-50'
          ]"
        >
          上一页
        </button>

        <div class="flex gap-1">
          <button
            v-for="page in displayPages"
            :key="page"
            @click="page !== '...' && handlePageChange(page)"
            :class="[
              'px-4 py-2 rounded-lg transition-all',
              page === currentPage
                ? 'bg-gradient-to-r from-hailong-primary to-hailong-secondary text-white'
                : page === '...'
                ? 'text-gray-400 cursor-default'
                : 'text-gray-700 hover:bg-gray-50 border border-gray-300'
            ]"
          >
            {{ page }}
          </button>
        </div>

        <button
          @click="handlePageChange(currentPage + 1)"
          :disabled="currentPage === totalPages"
          :class="[
            'px-4 py-2 rounded-lg border transition-all',
            currentPage === totalPages
              ? 'border-gray-200 text-gray-400 cursor-not-allowed'
              : 'border-gray-300 text-gray-700 hover:bg-gray-50'
          ]"
        >
          下一页
        </button>
      </div>
        </div>
      </div>
    </div>

    <!-- 内容区域 - 详情 -->
    <div v-else class="py-16 bg-white">
      <AnnouncementDetail :hashId="selectedHashId" :embedded="true" @back="selectedHashId = null" />
    </div>

    <Footer />
  </div>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue'
import { useRouter, useRoute } from 'vue-router'
import Header from '@/components/Header.vue'
import Footer from '@/components/Footer.vue'
import AnnouncementDetail from '@/views/details/AnnouncementDetail.vue'
import { getAnnouncementList } from '@/api/announcement.js'
import { getProvinceList, getCityList, getDistrictList } from '@/api/region.js'

const router = useRouter()
const route = useRoute()

// 业务类型选项
const businessTypes = [
  { label: '全部', value: '' },
  { label: '政府采购', value: 'GOV_PROCUREMENT' },
  { label: '建设工程', value: 'CONSTRUCTION' }
]

// 公告类型选项 - 全部业务类型
const announcementTypesAll = [
  { label: '全部', value: '' },
  { label: '招标/采购公告', value: 'bidding' },
  { label: '更正公告', value: 'correction' },
  { label: '结果公告', value: 'result' }
]

// 公告类型选项 - 政府采购
const announcementTypesGov = [
  { label: '全部', value: '' },
  { label: '招标公告', value: 'bidding' },
  { label: '更正公告', value: 'correction' },
  { label: '结果公告', value: 'result' }
]

// 公告类型选项 - 建设工程
const announcementTypesConstruction = [
  { label: '全部', value: '' },
  { label: '采购公告', value: 'bidding' },
  { label: '更正公告', value: 'correction' },
  { label: '结果公告', value: 'result' }
]

// 采购类型选项（仅政府采购时显示）
const procurementTypes = [
  { label: '全部', value: '' },
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
  { label: '全部', value: '' },
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
const selectedHashId = ref(null)

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
    // 根据省份名称找到省份代码
    const province = provinces.value.find(p => p.regionName === searchParams.value.province)
    if (province) {
      selectedProvinceCode.value = province.regionCode
      await loadCities(province.regionCode)
    }
  } else {
    selectedProvinceCode.value = ''
  }
}

// 城市变化处理
const onCityChange = async () => {
  searchParams.value.district = ''
  districts.value = []
  
  if (searchParams.value.city) {
    // 根据城市名称找到城市代码
    const city = cities.value.find(c => c.regionName === searchParams.value.city)
    if (city) {
      selectedCityCode.value = city.regionCode
      await loadDistricts(city.regionCode)
    }
  } else {
    selectedCityCode.value = ''
  }
}

// 业务类型变化处理
const handleBusinessTypeChange = (value) => {
  searchParams.value.businessType = value
  // 如果不是政府采购，清空采购类型
  if (value !== 'GOV_PROCUREMENT') {
    searchParams.value.procurementType = ''
  }
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
}

// 自定义日期变化
const onCustomDateChange = () => {
  searchParams.value.timeRange = 'custom'
}

// 加载公告列表
const loadAnnouncements = async () => {
  loading.value = true
  try {
    // 构建查询参数，只传递有值的参数
    const params = {
      pageNumber: currentPage.value,
      pageSize: pageSize.value
    }
    
    // 只添加非空参数
    if (searchParams.value.businessType) {
      params.businessType = searchParams.value.businessType
    }
    if (searchParams.value.noticeType) {
      params.noticeType = searchParams.value.noticeType
    }
    // 只有选择政府采购时，才传递采购类型
    if (searchParams.value.businessType === 'GOV_PROCUREMENT' && searchParams.value.procurementType) {
      params.procurementType = searchParams.value.procurementType
    }
    
    // 传递地区名称（数据库存储的是名称字符串）
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
    
    console.log('查询参数:', params) // 调试日志
    
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
  loadAnnouncements()
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
  loadAnnouncements()
}

// 查看详情
const handleViewDetail = (hashId) => {
  selectedHashId.value = hashId
  window.scrollTo({ top: 0, behavior: 'smooth' })
}

// 分页变化
const handlePageChange = (page) => {
  currentPage.value = page
  loadAnnouncements()
  // 滚动到顶部
  window.scrollTo({ top: 0, behavior: 'smooth' })
}

// 计算总页数
const totalPages = computed(() => {
  return Math.ceil(total.value / pageSize.value)
})

// 计算显示的页码
const displayPages = computed(() => {
  const pages = []
  const total = totalPages.value
  const current = currentPage.value
  
  if (total <= 7) {
    for (let i = 1; i <= total; i++) {
      pages.push(i)
    }
  } else {
    if (current <= 4) {
      for (let i = 1; i <= 5; i++) {
        pages.push(i)
      }
      pages.push('...')
      pages.push(total)
    } else if (current >= total - 3) {
      pages.push(1)
      pages.push('...')
      for (let i = total - 4; i <= total; i++) {
        pages.push(i)
      }
    } else {
      pages.push(1)
      pages.push('...')
      for (let i = current - 1; i <= current + 1; i++) {
        pages.push(i)
      }
      pages.push('...')
      pages.push(total)
    }
  }
  
  return pages
})

import { formatDate } from '@/utils/date'

// 获取类型样式
const getTypeStyle = (typeName) => {
  if (typeName?.includes('招标') || typeName?.includes('采购')) {
    return 'bg-blue-50 text-blue-600'
  } else if (typeName?.includes('结果') || typeName?.includes('中标')) {
    return 'bg-green-50 text-green-600'
  } else if (typeName?.includes('更正') || typeName?.includes('变更')) {
    return 'bg-orange-50 text-orange-600'
  }
  return 'bg-gray-50 text-gray-600'
}

// 初始化
onMounted(async () => {
  await loadProvinces()
  await loadAnnouncements()
})
</script>

<style scoped>
.container-wide {
  max-width: 1400px;
  margin: 0 auto;
  padding: 0 2rem;
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

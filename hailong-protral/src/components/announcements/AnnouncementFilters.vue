<template>
<div class="bg-white/80 backdrop-blur-md rounded-2xl shadow-[0_12px_40px_rgba(30,41,59,0.03)] border border-slate-100/80 p-6 mb-6">
        <!-- 关键字搜索 - 主搜索框 -->
        <div class="mb-5">
          <div class="flex flex-wrap gap-3">
            <div class="relative w-full sm:w-auto sm:flex-1 group">
              <svg class="absolute left-4 top-1/2 transform -translate-y-1/2 w-5 h-5 text-slate-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M21 21l-6-6m2-5a7 7 0 11-14 0 7 7 0 0114 0z" />
              </svg>
              <input
                v-model="searchParams.keyword" aria-label="搜索公告"
                type="text"
                placeholder="请输入项目名称、招标单位等关键字"
                class="w-full pl-12 pr-10 py-3 border border-slate-200 rounded-xl focus:ring-2 focus:ring-hailong-primary/20 focus:border-hailong-primary outline-none transition-all text-sm hover:border-slate-300 bg-slate-50/50 focus:bg-white focus:shadow-[0_4px_20px_rgba(40,120,255,0.06)]"
                @keyup.enter="handleSearch"
              />
              <!-- 一键清除按钮 -->
              <button
                v-if="searchParams.keyword"
                @click="clearKeyword"
                class="absolute right-3.5 top-1/2 transform -translate-y-1/2 text-slate-400 hover:text-slate-600 hover:scale-110 active:scale-95 transition-all focus:outline-none"
                title="清除关键字"
              >
                <svg class="w-4 h-4 transition-transform hover:rotate-90 duration-200" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12" />
                </svg>
              </button>
            </div>
            <button
              @click="handleSearch"
              class="portal-button portal-button--primary px-8 py-3 text-sm"
            >
              搜索
            </button>
            <button
              @click="handleReset"
              class="portal-button portal-button--secondary px-6 py-3 text-sm"
            >
              重置
            </button>
          </div>
        </div>

        <!-- 筛选条件 -->
        <div class="space-y-4 pt-3 border-t border-slate-100">
          <!-- 业务类型和采购类型 -->
          <div class="flex items-center gap-4 flex-wrap pb-4 border-b border-dashed border-slate-100">
            <label class="text-xs font-bold text-slate-400 whitespace-nowrap w-16">业务类型</label>
            <button
              v-for="type in businessTypes"
              :key="type.value"
              @click="handleBusinessTypeChange(type.value)"
              :class="[
                'portal-choice px-4 py-1.5 text-xs',
                searchParams.businessType === type.value
                  ? 'portal-choice--selected'
                  : ''
              ]"
            >
              {{ type.label }}
            </button>

            <!-- 采购类型 - 仅在选择政府采购时显示 -->
            <template v-if="searchParams.businessType === 'GOV_PROCUREMENT'">
              <label class="text-xs font-bold text-slate-400 whitespace-nowrap w-16 ml-6">采购类型</label>
              <button
                v-for="type in procurementTypes"
                :key="type.value"
                @click="handleProcurementTypeChange(type.value)"
                :class="[
                  'portal-choice px-4 py-1.5 text-xs',
                  searchParams.procurementType === type.value
                    ? 'portal-choice--selected'
                    : ''
                ]"
              >
                {{ type.label }}
              </button>
            </template>
          </div>

          <!-- 公告类型 -->
          <div class="flex items-center gap-4 pb-4 border-b border-dashed border-slate-100">
            <label class="text-xs font-bold text-slate-400 whitespace-nowrap w-16">公告类型</label>
            <div class="flex flex-wrap gap-2">
              <button
                v-for="type in currentAnnouncementTypes"
                :key="type.value"
                @click="handleNoticeTypeChange(type.value)"
                :class="[
                  'portal-choice px-4 py-1.5 text-xs',
                  searchParams.noticeType === type.value
                    ? 'portal-choice--selected'
                    : ''
                ]"
              >
                {{ type.label }}
              </button>
            </div>
          </div>

          <!-- 项目区域和发布时间 -->
          <div class="flex items-center gap-4 flex-wrap">
            <div class="flex flex-wrap items-center gap-4">
              <label class="text-xs font-bold text-slate-400 whitespace-nowrap w-16">项目区域</label>

              <!-- 省份选择 -->
              <div class="relative">
                <svg class="absolute left-3 top-1/2 transform -translate-y-1/2 w-4 h-4 text-slate-400 pointer-events-none" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M17.657 16.657L13.414 20.9a1.998 1.998 0 01-2.827 0l-4.244-4.243a8 8 0 1111.314 0z" />
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M15 11a3 3 0 11-6 0 3 3 0 016 0z" />
                </svg>
                <select
                  v-model="searchParams.province" aria-label="省份"
                  @change="onProvinceChange"
                  class="pl-9 pr-8 py-1.5 border border-slate-200 rounded-xl focus:ring-2 focus:ring-hailong-primary/20 focus:border-hailong-primary outline-none transition-all text-xs font-semibold hover:border-slate-300 bg-white cursor-pointer appearance-none"
                  style="min-width: 120px;"
                >
                  <option value="">全部省份 ({{ regionCounts.provinceTotal }})</option>
                  <option v-for="province in provinces" :key="province.regionCode" :value="province.regionCode">
                    {{ province.regionName }} ({{ province.count }})
                  </option>
                  <option v-if="regionCounts.provinceUnlocated > 0" disabled>
                    省份未明确 ({{ regionCounts.provinceUnlocated }})
                  </option>
                </select>
                <svg class="absolute right-3 top-1/2 transform -translate-y-1/2 w-4 h-4 text-slate-400 pointer-events-none" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 9l-7 7-7-7" />
                </svg>
              </div>

              <!-- 城市选择 -->
              <div class="relative" v-if="searchParams.province">
                <svg class="absolute left-3 top-1/2 transform -translate-y-1/2 w-4 h-4 text-slate-400 pointer-events-none" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 21V5a2 2 0 00-2-2H7a2 2 0 00-2 2v16m14 0h2m-2 0h-5m-9 0H3m2 0h5M9 7h1m-1 4h1m4-4h1m-1 4h1m-5 10v-5a1 1 0 011-1h2a1 1 0 011 1v5m-4 0h4" />
                </svg>
                <select
                  v-model="searchParams.city" aria-label="城市"
                  @change="onCityChange"
                  class="pl-9 pr-8 py-1.5 border border-slate-200 rounded-xl focus:ring-2 focus:ring-hailong-primary/20 focus:border-hailong-primary outline-none transition-all text-xs font-semibold hover:border-slate-300 bg-white cursor-pointer appearance-none"
                  style="min-width: 120px;"
                >
                  <option value="">全部城市 ({{ regionCounts.cityTotal }})</option>
                  <option v-for="city in cities" :key="city.regionCode" :value="city.regionCode">
                    {{ city.regionName }} ({{ city.count }})
                  </option>
                  <option v-if="regionCounts.cityUnlocated > 0" disabled>
                    城市未明确 ({{ regionCounts.cityUnlocated }})
                  </option>
                </select>
                <svg class="absolute right-3 top-1/2 transform -translate-y-1/2 w-4 h-4 text-slate-400 pointer-events-none" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 9l-7 7-7-7" />
                </svg>
              </div>

              <!-- 区县选择 -->
              <div class="relative" v-if="searchParams.city">
                <svg class="absolute left-3 top-1/2 transform -translate-y-1/2 w-4 h-4 text-slate-400 pointer-events-none" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M3 12l2-2m0 0l7-7 7 7M5 10v10a1 1 0 001 1h3m10-11l2 2m-2-2v10a1 1 0 01-1 1h-3m-6 0a1 1 0 001-1v-4a1 1 0 011-1h2a1 1 0 011 1v4a1 1 0 001 1m-6 0h6" />
                </svg>
                <select
                  v-model="searchParams.district" aria-label="区县"
                  @change="onDistrictChange"
                  class="pl-9 pr-8 py-1.5 border border-slate-200 rounded-xl focus:ring-2 focus:ring-hailong-primary/20 focus:border-hailong-primary outline-none transition-all text-xs font-semibold hover:border-slate-300 bg-white cursor-pointer appearance-none"
                  style="min-width: 120px;"
                >
                  <option value="">全部区县 ({{ regionCounts.districtTotal }})</option>
                  <option v-for="district in districts" :key="district.regionCode" :value="district.regionCode">
                    {{ district.regionName }} ({{ district.count }})
                  </option>
                  <option v-if="regionCounts.districtUnlocated > 0" disabled>
                    区县未明确 ({{ regionCounts.districtUnlocated }})
                  </option>
                </select>
                <svg class="absolute right-3 top-1/2 transform -translate-y-1/2 w-4 h-4 text-slate-400 pointer-events-none" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 9l-7 7-7-7" />
                </svg>
              </div>
            </div>

            <label class="text-xs font-bold text-slate-400 whitespace-nowrap w-16 ml-2">发布时间</label>
            <button
              v-for="time in timeRanges"
              :key="time.value"
              @click="selectTimeRange(time.value)"
              :class="[
                'portal-choice px-4 py-1.5 text-xs',
                searchParams.timeRange === time.value
                  ? 'portal-choice--selected'
                  : ''
              ]"
            >
              {{ time.label }}
            </button>
            <button
              @click="showCustomDatePicker = !showCustomDatePicker"
              :class="[
                'portal-choice px-4 py-1.5 text-xs flex items-center gap-1',
                searchParams.timeRange === 'custom'
                  ? 'portal-choice--selected'
                  : ''
              ]"
            >
              <svg class="w-3.5 h-3.5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M8 7V3m8 4V3m-9 8h10M5 21h14a2 2 0 002-2V7a2 2 0 00-2-2H5a2 2 0 00-2 2v12a2 2 0 002 2z" />
              </svg>
              自定义
            </button>

            <!-- 自定义日期选择器 -->
            <template v-if="showCustomDatePicker">
              <div class="relative">
                <svg class="absolute left-3 top-1/2 transform -translate-y-1/2 w-4 h-4 text-slate-400 pointer-events-none" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M8 7V3m8 4V3m-9 8h10M5 21h14a2 2 0 002-2V7a2 2 0 00-2-2H5a2 2 0 00-2 2v12a2 2 0 002 2z" />
                </svg>
                <input
                  type="date"
                  v-model="searchParams.startDate" aria-label="开始日期"
                  @change="onCustomDateChange"
                  class="pl-9 pr-3 py-1.5 border border-slate-200 rounded-xl focus:ring-2 focus:ring-hailong-primary/20 focus:border-hailong-primary outline-none transition-all text-xs font-semibold hover:border-slate-300 bg-white"
                />
              </div>
              <span class="text-slate-400 text-xs font-bold self-center">至</span>
              <div class="relative">
                <svg class="absolute left-3 top-1/2 transform -translate-y-1/2 w-4 h-4 text-slate-400 pointer-events-none" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M8 7V3m8 4V3m-9 8h10M5 21h14a2 2 0 002-2V7a2 2 0 00-2-2H5a2 2 0 00-2 2v12a2 2 0 002 2z" />
                </svg>
                <input
                  type="date"
                  v-model="searchParams.endDate" aria-label="结束日期"
                  @change="onCustomDateChange"
                  class="pl-9 pr-3 py-1.5 border border-slate-200 rounded-xl focus:ring-2 focus:ring-hailong-primary/20 focus:border-hailong-primary outline-none transition-all text-xs font-semibold hover:border-slate-300 bg-white"
                />
              </div>
            </template>
          </div>
        </div>
      </div>
</template>
<script setup>
import { toRefs } from 'vue'
const props = defineProps({ controller: { type: Object, required: true } })
const { searchParams, showCustomDatePicker, provinces, cities, districts, regionCounts, businessTypes, currentAnnouncementTypes, procurementTypes, timeRanges, handleSearch, clearKeyword, handleReset, handleBusinessTypeChange, handleNoticeTypeChange, handleProcurementTypeChange, onProvinceChange, onCityChange, onDistrictChange, selectTimeRange, onCustomDateChange } = toRefs(props.controller)
</script>

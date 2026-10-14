<template>
<div
          @click="$emit('open', announcement.hashId || announcement.id)" @keydown.enter="$emit('open', announcement.hashId || announcement.id)" role="link" tabindex="0"
          :class="getAnnouncementTone(announcement.noticeType)"
          class="announcement-card group bg-white rounded-2xl p-6 shadow-[0_8px_30px_rgb(0,0,0,0.02)] hover:shadow-sm border border-slate-100 transition-shadow duration-200 cursor-pointer flex flex-col gap-4"
        >
          <!-- 公告类型为主标签，业务与采购分类为辅助信息。 -->
          <div class="flex flex-wrap items-center justify-between gap-x-3 gap-y-2">
            <span class="announcement-tag">{{ announcement.noticeTypeName }}</span>
            <span class="ml-auto whitespace-nowrap text-[13px] leading-5 text-[#64748b]">{{ getAnnouncementClassification(announcement) }}</span>
          </div>

          <!-- 标题 -->
          <h3 class="text-base md:text-lg font-bold text-slate-800 leading-snug group-hover:text-hailong-primary transition-colors line-clamp-2">
            {{ announcement.title }}
          </h3>

          <!-- 中标人信息 -->
          <div v-if="announcement.winner" class="p-3.5 bg-gradient-to-r from-emerald-50/40 via-teal-50/15 to-transparent border-l-4 border-emerald-500 rounded-r-xl rounded-l-md shadow-[sm_0_2px_8px_rgba(16,185,129,0.02)] flex items-center justify-between gap-4">
            <div class="flex items-center gap-2 min-w-0">
              <div class="w-7 h-7 rounded-lg bg-emerald-500/10 flex items-center justify-center shrink-0">
                <svg class="w-4 h-4 text-emerald-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2.5" d="M9 12l2 2 4-4m6 2a9 9 0 11-18 0 9 9 0 0118 0z" />
                </svg>
              </div>
              <span class="text-xs text-slate-400 font-bold shrink-0">中标人：</span>
              <span class="text-sm text-emerald-800 font-extrabold truncate">{{ announcement.winner }}</span>
            </div>
          </div>

          <!-- 其他详细信息 -->
          <div class="grid grid-cols-1 md:grid-cols-3 gap-3 text-xs md:text-sm text-slate-500">
            <div v-if="announcement.bidder" class="flex items-center gap-1.5 min-w-0">
              <svg class="w-4 h-4 text-slate-400 group-hover:text-hailong-primary transition-colors shrink-0" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 21V5a2 2 0 00-2-2H7a2 2 0 00-2 2v16m14 0h2m-2 0h-5m-9 0H3m2 0h5M9 7h1m-1 4h1m4-4h1m-1 4h1m-5 10v-5a1 1 0 011-1h2a1 1 0 011 1v5m-4 0h4" />
              </svg>
              <span class="text-slate-400 whitespace-nowrap">招标人：</span>
              <span class="text-slate-600 font-medium truncate">{{ announcement.bidder }}</span>
            </div>
            <div v-if="announcement.projectRegion" class="flex items-center gap-1.5">
              <svg class="w-4 h-4 text-slate-400 group-hover:text-hailong-secondary transition-colors shrink-0" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M17.657 16.657L13.414 20.9a1.998 1.998 0 01-2.827 0l-4.244-4.243a8 8 0 1111.314 0z" />
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M15 11a3 3 0 11-6 0 3 3 0 016 0z" />
              </svg>
              <span class="text-slate-400 whitespace-nowrap">项目区域：</span>
              <span class="text-slate-600 font-medium">{{ announcement.projectRegion }}</span>
            </div>
            <div v-if="announcement.publishTime" class="flex items-center gap-1.5">
              <svg class="w-4 h-4 text-slate-400 group-hover:text-blue-500 transition-colors shrink-0" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M8 7V3m8 4V3m-9 8h10M5 21h14a2 2 0 002-2V7a2 2 0 00-2-2H5a2 2 0 00-2 2v12a2 2 0 002 2z" />
              </svg>
              <span class="text-slate-400 whitespace-nowrap">发布时间：</span>
              <span class="text-slate-600 font-medium">{{ formatDate(announcement.publishTime) }}</span>
            </div>
          </div>

          <!-- 底部信息 -->
          <div class="flex items-center justify-between pt-3 border-t border-slate-100/60 text-xs text-slate-400">
            <div class="flex items-center gap-4">
              <span class="flex items-center gap-1">
                <svg class="w-3.5 h-3.5 text-slate-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M15 12a3 3 0 11-6 0 3 3 0 016 0z" />
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M2.458 12C3.732 7.943 7.523 5 12 5c4.478 0 8.268 2.943 9.542 7-1.274 4.057-5.064 7-9.542 7-4.477 0-8.268-2.943-9.542-7z" />
                </svg>
                {{ announcement.viewCount || 0 }} 次浏览
              </span>
            </div>
            <span class="text-hailong-primary text-sm font-bold flex items-center gap-1 group-hover:translate-x-1 transition-all">
              查看详情 <svg class="w-3.5 h-3.5" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 5l7 7-7 7"/></svg>
            </span>
          </div>
        </div>
</template>
<script setup>
import { formatDate } from '@/utils/date'
import { getAnnouncementTone, getAnnouncementClassification } from '@/utils/announcementAppearance'
defineProps({ announcement: { type: Object, required: true } })
defineEmits(['open'])
</script>

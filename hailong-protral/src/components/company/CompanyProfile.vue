<template>
  <section class="mx-auto max-w-5xl overflow-hidden rounded-2xl border border-slate-100 bg-white shadow-sm">
    <AsyncState v-if="loading || error || !profile" :loading="loading" :error="error" empty-text="暂无企业简介" @retry="$emit('retry')" />
    <template v-else>
      <button v-if="profile.imageUrl" type="button" class="block w-full" aria-label="查看企业图片" @click="$emit('preview', profile.imageUrl)"><img :src="profile.imageUrl" alt="关于海隆" class="max-h-96 w-full object-cover" /></button>
      <div class="p-6 md:p-10">
        <h2 class="mb-6 text-2xl font-bold text-slate-800">{{ profile.title || '企业简介' }}</h2>
        <RichTextContent :content="profile.content" />
      </div>
    </template>
  </section>
</template>
<script setup>
import AsyncState from '@/components/common/AsyncState.vue'
import RichTextContent from '@/components/common/RichTextContent.vue'
defineProps({ profile: Object, loading: Boolean, error: String })
defineEmits(['retry', 'preview'])
</script>

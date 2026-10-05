<template>
  <section>
    <h2 class="mb-8 text-center text-3xl font-bold text-slate-800">{{ title }}</h2>
    <AsyncState v-if="loading || error || !items.length" :loading="loading" :error="error" :empty-text="`暂无${title}内容`" @retry="$emit('retry')" />
    <div v-else class="grid grid-cols-1 gap-6 md:grid-cols-2 lg:grid-cols-3">
      <component :is="item.to ? RouterLink : 'button'" v-for="item in items" :key="item.id" :to="item.to" :type="item.to ? undefined : 'button'" @click="!item.to && item.image && $emit('preview', item.image)" class="group overflow-hidden rounded-2xl border border-slate-100 bg-white text-left shadow-sm transition-shadow hover:shadow-lg">
        <div v-if="item.image" class="overflow-hidden bg-slate-100" :class="certificateImages ? 'h-72 p-4' : 'h-48'"><img :src="item.image" :alt="item.title" loading="lazy" class="h-full w-full" :class="certificateImages ? 'object-contain' : 'object-cover'" /></div>
        <div class="p-6">
          <h3 class="mb-3 text-xl font-bold text-slate-800 group-hover:text-hailong-primary">{{ item.title }}</h3>
          <p v-if="item.description" class="mb-4 text-sm leading-relaxed text-slate-600">{{ item.description }}</p>
          <ul v-if="item.features?.length" class="space-y-2 text-sm text-slate-500"><li v-for="feature in item.features" :key="feature">• {{ feature }}</li></ul>
        </div>
      </component>
    </div>
  </section>
</template>
<script setup>
import { RouterLink } from 'vue-router'
import AsyncState from '@/components/common/AsyncState.vue'
defineProps({ title: String, items: { type: Array, default: () => [] }, certificateImages: Boolean, loading: Boolean, error: String })
defineEmits(['retry', 'preview'])
</script>

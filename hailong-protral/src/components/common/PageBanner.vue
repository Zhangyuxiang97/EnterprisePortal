<template>
  <header class="portal-page-banner relative h-[280px] overflow-hidden bg-gradient-to-br from-hailong-dark via-slate-900 to-indigo-950 px-6 pt-32 text-center text-white md:h-[300px]">
    <div class="banner-grid pointer-events-none absolute inset-0 opacity-10" aria-hidden="true"></div>
    <div class="pointer-events-none absolute -top-40 left-1/2 h-[600px] w-[600px] -translate-x-1/2 animate-float rounded-full bg-hailong-primary/10 blur-[120px] motion-reduce:animate-none" aria-hidden="true"></div>
    <div class="relative z-10 mx-auto w-full max-w-4xl">
      <h1 class="mb-4 bg-gradient-to-r from-white via-slate-100 to-slate-200 bg-clip-text font-tech text-4xl font-extrabold tracking-tight text-transparent md:text-5xl">{{ title }}</h1>
      <p class="mx-auto max-w-2xl text-base font-medium leading-6 text-slate-300 md:text-lg md:leading-7">{{ description }}</p>
    </div>
  </header>
</template>

<script setup>
defineProps({
  title: { type: String, required: true },
  description: { type: String, required: true }
})
</script>

<style scoped>
.banner-grid {
  background-image: linear-gradient(to right, rgba(255,255,255,0.05) 1px, transparent 1px),
                    linear-gradient(to bottom, rgba(255,255,255,0.05) 1px, transparent 1px);
  background-size: 24px 24px;
}
</style>

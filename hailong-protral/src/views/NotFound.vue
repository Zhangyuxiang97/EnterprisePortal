<template>
  <section class="min-h-[70vh] px-6 pt-40 pb-20 text-center bg-slate-50">
    <p class="text-hailong-primary text-6xl font-bold">404</p>
    <h1 class="mt-6 text-2xl font-bold text-slate-800">页面不存在</h1>
    <p class="mt-3 text-slate-500">地址可能有误，或内容已经移动。您可以从公告栏目继续查找。</p>
    <div class="mt-8 flex justify-center gap-4">
      <router-link to="/announcements" class="rounded-lg bg-hailong-primary px-5 py-3 text-white">查询公告</router-link>
      <router-link to="/" class="rounded-lg border border-slate-300 px-5 py-3 text-slate-700">返回首页</router-link>
    </div>
  </section>
</template>

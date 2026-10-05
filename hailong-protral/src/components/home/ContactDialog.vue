<template>
<dialog ref="dialog" aria-labelledby="contact-dialog-title" @close="$emit('close')" @click="closeOnBackdrop" class="m-auto w-[calc(100%_-_2rem)] max-w-2xl rounded-2xl bg-transparent p-0">
      <div @click.stop class="bg-white rounded-2xl shadow-2xl max-w-2xl w-full max-h-[90vh] overflow-y-auto">
        <!-- 模态框头部 -->
        <div class="bg-gradient-to-r from-hailong-primary to-hailong-secondary p-6 text-white relative">
          <button aria-label="关闭联系信息" @click="dialog.close()"
            class="absolute top-4 right-4 text-white hover:text-gray-200 transition-colors">
            <svg class="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12"></path>
            </svg>
          </button>
          <h3 id="contact-dialog-title" class="text-2xl font-bold">联系我们</h3>
          <p class="text-white/80 mt-2">期待与您的合作，共创美好未来</p>
        </div>

        <!-- 模态框内容 -->
        <div class="p-8">
          <div class="grid grid-cols-1 md:grid-cols-2 gap-6 items-stretch">
            <!-- 联系方式列表 -->
            <div class="space-y-6 flex flex-col justify-center">
              <div class="flex items-start">
                <div
                  class="w-12 h-12 bg-hailong-primary/10 rounded-full flex items-center justify-center mr-4 flex-shrink-0">
                  <svg class="w-6 h-6 text-hailong-primary" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                      d="M3 5a2 2 0 012-2h3.28a1 1 0 01.948.684l1.498 4.493a1 1 0 01-.502 1.21l-2.257 1.13a11.042 11.042 0 005.516 5.516l1.13-2.257a1 1 0 011.21-.502l4.493 1.498a1 1 0 01.684.949V19a2 2 0 01-2 2h-1C9.716 21 3 14.284 3 6V5z">
                    </path>
                  </svg>
                </div>
                <div>
                  <h4 class="font-bold text-gray-900 mb-1">联系电话</h4>
                  <p class="text-gray-600 text-sm">{{ contactInfo.phone }}</p>
                </div>
              </div>

              <div class="flex items-start">
                <div
                  class="w-12 h-12 bg-hailong-primary/10 rounded-full flex items-center justify-center mr-4 flex-shrink-0">
                  <svg class="w-6 h-6 text-hailong-primary" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                      d="M3 8l7.89 5.26a2 2 0 002.22 0L21 8M5 19h14a2 2 0 002-2V7a2 2 0 00-2-2H5a2 2 0 00-2 2v10a2 2 0 002 2z">
                    </path>
                  </svg>
                </div>
                <div>
                  <h4 class="font-bold text-gray-900 mb-1">邮箱地址</h4>
                  <p class="text-gray-600 text-sm break-all">{{ contactInfo.email }}</p>
                </div>
              </div>

              <div class="flex items-start">
                <div
                  class="w-12 h-12 bg-hailong-primary/10 rounded-full flex items-center justify-center mr-4 flex-shrink-0">
                  <svg class="w-6 h-6 text-hailong-primary" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                      d="M17.657 16.657L13.414 20.9a1.998 1.998 0 01-2.827 0l-4.244-4.243a8 8 0 1111.314 0z"></path>
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                      d="M15 11a3 3 0 11-6 0 3 3 0 016 0z"></path>
                  </svg>
                </div>
                <div>
                  <h4 class="font-bold text-gray-900 mb-1">公司地址</h4>
                  <p class="text-gray-600 text-sm">{{ contactInfo.address.fullAddress }}</p>
                </div>
              </div>
            </div>

            <!-- 工作时间 -->
            <div
              class="flex flex-col items-center justify-center bg-gradient-to-br from-hailong-primary/10 to-hailong-secondary/10 rounded-xl p-6 h-full">
              <div class="w-20 h-20 bg-hailong-primary/20 rounded-full flex items-center justify-center mb-4">
                <svg class="w-10 h-10 text-hailong-primary" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                    d="M12 8v4l3 3m6-3a9 9 0 11-18 0 9 9 0 0118 0z"></path>
                </svg>
              </div>
              <h4 class="font-bold text-gray-900 text-lg mb-2">工作时间</h4>
              <p class="text-hailong-primary text-xl font-bold mb-3">{{ contactInfo.workingHours.weekdays }}</p>
              <p class="text-gray-500 text-sm text-center">节假日{{ contactInfo.workingHours.weekend }}</p>
            </div>
          </div>
        </div>
      </div>
    </dialog>
</template>
<script setup>
import { ref, computed, onMounted } from 'vue'
import { getContactInfo } from '@/utils/config'
defineEmits(['close'])
const dialog = ref(null)
const contactInfo = computed(getContactInfo)
const closeOnBackdrop = event => { if (event.target === dialog.value) dialog.value.close() }
onMounted(() => dialog.value.showModal())
</script>
<style scoped>
dialog::backdrop { background: rgb(0 0 0 / .6); backdrop-filter: blur(4px); }
</style>

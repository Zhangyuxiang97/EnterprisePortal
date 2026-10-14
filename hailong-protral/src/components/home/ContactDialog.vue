<template>
<dialog ref="dialog" aria-labelledby="contact-dialog-title" aria-describedby="contact-dialog-description" @close="$emit('close')" @click="closeOnBackdrop" class="contact-dialog m-auto w-[calc(100%_-_2rem)] max-w-2xl rounded-2xl border border-slate-200 bg-white p-0 text-slate-800 shadow-2xl">
      <div @click.stop class="flex max-h-[calc(100dvh_-_2rem)] flex-col">
        <!-- 模态框头部 -->
        <header class="portal-tint portal-tone-blue flex shrink-0 items-start justify-between gap-4 border-b px-5 py-5 sm:px-7 sm:py-6">
          <div class="min-w-0">
            <h3 id="contact-dialog-title" class="text-xl font-semibold tracking-tight text-slate-900 sm:text-2xl">联系我们</h3>
            <p id="contact-dialog-description" class="mt-2 text-sm leading-relaxed text-slate-500">通过电话或邮件，与我们沟通您的业务需求。</p>
          </div>
          <button type="button" aria-label="关闭联系信息" @click="dialog.close()"
            class="contact-close flex h-11 w-11 shrink-0 items-center justify-center rounded-lg text-slate-500 transition-colors hover:bg-slate-100 hover:text-slate-800">
            <svg aria-hidden="true" class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12"></path>
            </svg>
          </button>
        </header>

        <!-- 模态框内容 -->
        <div class="min-h-0 overflow-y-auto px-5 py-6 sm:px-7 sm:py-7">
          <div class="grid min-w-0 grid-cols-1 gap-6 sm:grid-cols-[minmax(0,1.35fr)_minmax(0,1fr)]">
            <!-- 联系方式列表 -->
            <div class="min-w-0 space-y-6">
              <div class="flex items-start">
                <div
                  class="contact-icon portal-tone-blue">
                  <svg aria-hidden="true" class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                      d="M3 5a2 2 0 012-2h3.28a1 1 0 01.948.684l1.498 4.493a1 1 0 01-.502 1.21l-2.257 1.13a11.042 11.042 0 005.516 5.516l1.13-2.257a1 1 0 011.21-.502l4.493 1.498a1 1 0 01.684.949V19a2 2 0 01-2 2h-1C9.716 21 3 14.284 3 6V5z">
                    </path>
                  </svg>
                </div>
                <div class="min-w-0">
                  <h4 class="text-sm text-slate-500 mb-1">联系电话</h4>
                  <a v-if="isPhone" :href="`tel:${contactInfo.phone}`" class="contact-link inline-block text-xl font-semibold tracking-wide">{{ contactInfo.phone }}</a>
                  <span v-else class="inline-block select-text text-xl font-semibold tracking-wide text-[#245b85]">{{ contactInfo.phone }}</span>
                </div>
              </div>

              <div class="flex items-start">
                <div
                  class="contact-icon portal-tone-teal">
                  <svg aria-hidden="true" class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                      d="M3 8l7.89 5.26a2 2 0 002.22 0L21 8M5 19h14a2 2 0 002-2V7a2 2 0 00-2-2H5a2 2 0 00-2 2v10a2 2 0 002 2z">
                    </path>
                  </svg>
                </div>
                <div class="min-w-0">
                  <h4 class="text-sm text-slate-500 mb-1">邮箱地址</h4>
                  <a :href="`mailto:${contactInfo.email}`" class="contact-link inline-block text-sm font-medium leading-relaxed">{{ contactInfo.email }}</a>
                </div>
              </div>

              <div class="flex items-start">
                <div
                  class="contact-icon portal-tone-gold">
                  <svg aria-hidden="true" class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                      d="M17.657 16.657L13.414 20.9a1.998 1.998 0 01-2.827 0l-4.244-4.243a8 8 0 1111.314 0z"></path>
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                      d="M15 11a3 3 0 11-6 0 3 3 0 016 0z"></path>
                  </svg>
                </div>
                <div class="min-w-0">
                  <h4 class="text-sm text-slate-500 mb-1">公司地址</h4>
                  <p class="text-slate-700 text-sm leading-relaxed">{{ contactInfo.address.fullAddress }}</p>
                </div>
              </div>
            </div>

            <!-- 工作时间 -->
            <section aria-labelledby="contact-hours-title" class="portal-tint portal-tone-gold self-start rounded-xl border p-5">
              <div class="flex items-center gap-2 text-slate-600">
                <svg aria-hidden="true" class="w-5 h-5 shrink-0" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                    d="M12 8v4l3 3m6-3a9 9 0 11-18 0 9 9 0 0118 0z"></path>
                </svg>
                <h4 id="contact-hours-title" class="text-sm font-medium">工作时间</h4>
              </div>
              <p class="mt-3 text-base font-semibold leading-relaxed text-slate-800">{{ contactInfo.workingHours.weekdays }}</p>
              <p class="mt-2 text-sm leading-relaxed text-slate-500">{{ contactInfo.workingHours.weekend }}</p>
            </section>
          </div>
        </div>
        <footer class="flex shrink-0 flex-wrap items-center justify-end gap-3 border-t border-slate-200 px-5 py-4 sm:px-7">
          <p v-if="copyState === 'copied' || copyState === 'error'" role="status" class="w-full text-sm leading-relaxed text-slate-600">
            {{ copyState === 'copied' ? '电话号码已复制' : '复制失败，请选中上方号码手动复制。' }}
          </p>
          <router-link to="/contact" class="portal-button portal-button--secondary flex-1 px-4 py-2.5 text-sm sm:flex-none" @click="dialog.close()">查看交通指引</router-link>
          <a v-if="isPhone" :href="`tel:${contactInfo.phone}`" class="portal-button portal-button--primary flex-1 px-5 py-2.5 text-sm sm:flex-none">拨打电话</a>
          <button v-else ref="copyButton" type="button" :aria-busy="copyState === 'copying'" class="portal-button portal-button--primary flex-1 px-5 py-2.5 text-sm sm:flex-none" @click="copyPhone">
            {{ copyState === 'copied' ? '已复制' : copyState === 'copying' ? '复制中…' : '复制电话' }}
          </button>
        </footer>
      </div>
    </dialog>
</template>
<script setup>
import { ref, computed, onMounted } from 'vue'
import { getContactInfo } from '@/utils/config'
defineEmits(['close'])
const dialog = ref(null)
const contactInfo = computed(getContactInfo)
const isPhone = navigator.userAgentData?.mobile === true || /iPhone|iPod|Android.*Mobile|Windows Phone/i.test(navigator.userAgent)
const copyButton = ref(null)
const copyState = ref('idle')
const copyPhone = async () => {
  if (copyState.value === 'copying') return
  copyState.value = 'copying'
  const phone = contactInfo.value.phone
  try {
    if (!navigator.clipboard?.writeText) throw new Error('Clipboard unavailable')
    await navigator.clipboard.writeText(phone)
    copyState.value = 'copied'
  } catch {
    if (!dialog.value?.open) return
    const input = document.createElement('textarea')
    input.value = phone
    input.readOnly = true
    input.setAttribute('aria-label', '复制电话号码')
    input.style.cssText = 'position:fixed;opacity:0;pointer-events:none;'
    // 原生模态弹窗之外的元素不可交互，复制文本域须放在弹窗内。
    dialog.value.appendChild(input)
    try {
      input.focus()
      input.select()
      copyState.value = document.execCommand('copy') ? 'copied' : 'error'
    } catch { copyState.value = 'error' }
    finally { input.remove(); copyButton.value?.focus() }
  }
}
const closeOnBackdrop = event => { if (event.target === dialog.value) dialog.value.close() }
onMounted(() => dialog.value.showModal())
</script>
<style scoped>
.contact-dialog { max-height: calc(100dvh - 2rem); overflow: hidden; overflow-wrap: anywhere; }
.contact-dialog::backdrop { background: rgb(15 23 42 / .55); }
.contact-icon { display: flex; width: 40px; height: 40px; margin-right: 12px; flex-shrink: 0; align-items: center; justify-content: center; border-radius: 10px; background: var(--tone-soft); color: var(--tone); }
.contact-link { color: #245b85; }
.contact-link:hover { color: #193f5c; text-decoration: underline; text-underline-offset: 3px; }
.contact-link:focus-visible, .contact-close:focus-visible { outline: 2px solid #4f83b1; outline-offset: 3px; border-radius: 4px; }
@media (prefers-reduced-motion: reduce) { .contact-close { transition: none; } }
</style>

/* 独立静态演示：业务内容来自 data.js；风格、筛选与详情均在浏览器本地完成。 */
(() => {
  'use strict';
  const data = window.PORTAL_DEMO_DATA;
  const root = document.querySelector('#site-root');
  const dialog = document.querySelector('#detail-dialog');
  const content = document.querySelector('#dialog-content');
  const styles = {
    corporate: { name: '专业企业蓝', subtitle: '清晰的秩序，可信的专业感', description: '用海隆品牌蓝建立识别。非对称首屏、建筑立面、紧凑公告与明确的服务层级，适合企业品牌与公告查询兼顾的门户。', focus: '品牌识别 / 企业形象与公告平衡', visual: '海隆蓝、深海军蓝、冷白；结构线与较小圆角', layout: '文字与建筑并列首屏 → 最新公告 → 业务 → 企业 → 资讯', hero: ['海纳百川', '才望兼隆'] },
    editorial: { name: '建筑编辑风', subtitle: '让工程咨询有自己的气质', description: '暖白纸感、墨绿与衬线中文。用建筑出版物的排版语言组织内容，强调留白、线条和比例；服务以目录呈现，信息保留清楚的阅读路径。', focus: '行业气质 / 品牌记忆', visual: '暖白、墨绿、陶土色；中文宋体与大编号', layout: '编辑式首屏 → 索引式公告 → 业务目录 → 企业专栏 → 资讯', hero: ['海纳百川', '才望兼隆'] },
    minimal: { name: '克制极简', subtitle: '有呼吸感，也保留信息效率', description: '以精确的字号、间距、边界和微小交互建立质感。黑白主导，只在操作上使用少量品牌蓝；不靠光晕、玻璃和堆叠卡片制造视觉热闹。', focus: '阅读舒适 / 克制与精度', visual: '黑、白、浅灰；大留白、细分隔线与轻反馈', layout: '居中宣言 + 建筑全景 → 公告列表 → 服务 → 企业 → 资讯', hero: ['海纳百川', '才望兼隆'] },
    portal: { name: '公告优先', subtitle: '打开门户，就能找到项目', description: '更短的企业横幅，把搜索、政府采购与建设工程放到首屏。公告采用较高密度的行列表，服务和联系入口随手可见，适合经常查询公告的访问者。', focus: '查询效率 / 高频业务访问', visual: '机构蓝、白底、细灰线；紧凑行列表', layout: '简短企业横幅 → 搜索与公告 + 服务侧栏 → 企业 → 资讯', hero: ['海纳百川', '才望兼隆'] }
  };
  const services = [
    { name: '政府采购代理', text: '货物、工程与服务类采购全流程代理。', lines: ['采购需求编制', '招标文件制作', '开标评标组织', '合同签订指导'], icon: '<path d="M5 7h14v14H5zM9 7V3h6v4M9 11h6M9 15h6M9 19h3"/>' },
    { name: '建设工程招标', text: '施工、监理、设计等工程项目招标代理。', lines: ['工程量清单编制', '招标文件编制', '现场勘察组织', '评标专家抽取'], icon: '<path d="M3 21h18M6 21V7l6-4 6 4v14M10 21v-4h4v4M10 8h4M10 12h4"/>' },
    { name: '工程造价咨询', text: '项目预算、结算审核与全过程造价控制。', lines: ['工程预算编制', '工程结算审核', '造价司法鉴定', '全过程造价控制'], icon: '<path d="M6 3h12v18H6zM9 7h6M9 11h1m4 0h1m-6 4h1m4 0h1m-6 4h1m4 0h1"/>' },
    { name: '工程监理', text: '工程建设全过程质量、进度与安全管理。', lines: ['施工阶段监理', '质量安全控制', '进度投资管理', '竣工验收协助'], icon: '<path d="M12 3l8 4v6c0 4-8 8-8 8s-8-4-8-8V7zM8 12l3 3 5-6"/>' },
    { name: '项目管理', text: '从项目策划到实施、评价的管理咨询。', lines: ['项目策划', '项目实施管理', '项目后评价', 'PMC总承包管理'], icon: '<path d="M4 4h6v6H4zM14 14h6v6h-6zM14 4h6v6h-6zM4 14h6v6H4z"/>' },
    { name: '招标文件编制', text: '招标、资格预审与技术规范文件编制。', lines: ['招标文件编制', '资格预审文件', '技术规范编写', '评标办法设计'], icon: '<path d="M6 3h8l4 4v14H6zM14 3v5h4M9 12h6M9 16h6"/>' }
  ];
  const labels = { bidding: '招标公告', result: '结果公告', correction: '更正公告' };
  const businesses = { GOV_PROCUREMENT: '政府采购', CONSTRUCTION: '建设工程' };
  const procurement = { goods: '货物', service: '服务', project: '工程' };
  const escape = value => String(value ?? '').replace(/[&<>"']/g, char => ({ '&': '&amp;', '<': '&lt;', '>': '&gt;', '"': '&quot;', "'": '&#39;' }[char]));
  const arrow = '<span class="arrow" aria-hidden="true">↗</span>';
  const state = { style: 'corporate', business: 'all', type: 'all', query: '', expanded: false };
  let toastTimer;

  function readSavedStyle() {
    const url = new URL(window.location.href);
    let saved;
    try { saved = localStorage.getItem('hailong-design-style'); } catch {}
    const requested = url.searchParams.get('style') || saved;
    return styles[requested] ? requested : 'corporate';
  }

  function renderSite() {
    root.innerHTML = `
      <header class="site-header"><div class="shell header-inner">
        <a href="#home" class="brand" aria-label="海隆工程咨询首页"><img src="assets/logo.png" alt="" width="44" height="44"><span><strong>海隆工程咨询</strong><small>HAILONG CONSULTING</small></span></a>
        <nav class="main-nav" aria-label="门户导航"><a href="#home" class="active">首页</a><a href="#announcements">公告信息</a><a href="#services">业务范围</a><a href="#about">关于海隆</a><a href="#insights">资讯与政策</a></nav>
        <button class="contact-action" type="button" data-action="contact">联系我们 ${arrow}</button>
        <button type="button" class="menu-toggle" aria-label="打开导航菜单" aria-expanded="false">☰</button>
      </div></header>
      <main id="home">
        <section class="hero" aria-labelledby="hero-title"><div class="shell hero-grid">
          <div class="hero-copy"><p class="eyebrow"><span class="line"></span> 海隆工程咨询有限公司 <span class="hero-en">/ HAILONG</span></p>
            <h1 id="hero-title"><span class="slogan-line">海纳百川</span><span class="slogan-line accent-line">才望兼隆<span class="hero-period">。</span></span></h1>
            <p class="hero-description">${escape(data.company.description)}</p>
            <div class="hero-actions"><a class="button primary" href="#announcements">查阅项目公告 <span aria-hidden="true">→</span></a><a class="button secondary" href="#services">了解我们的服务 ${arrow}</a></div>
            <div class="hero-caption"><span class="small-square"></span> 以科学、规范、公正的服务，让项目有序向前。</div>
          </div>
          <figure class="hero-visual"><img src="assets/architecture.svg" width="1800" height="1000" alt="以建筑立面、结构和透视构成的原创矢量插画"><figcaption><span>STRUCTURE & PRECISION</span><span>结构 · 秩序 · 精度</span></figcaption><span class="visual-index" aria-hidden="true">HL / 01</span></figure>
          <div class="hero-bottom"><span>专业连接价值，规范支撑建设。</span><a href="#announcements" aria-label="向下查看公告信息">向下探索 <span aria-hidden="true">↓</span></a></div>
        </div></section>
        <div class="body-layout shell">
          <section class="announcements section" id="announcements" aria-labelledby="announcements-title">
            <div class="section-heading"><div><p class="eyebrow section-kicker">01 / INFORMATION</p><h2 id="announcements-title">项目公告<span class="heading-period">.</span></h2></div><p class="section-description">公开透明的信息，清晰有序的进展。</p></div>
            <div class="archive-tools"><div class="business-tabs" role="group" aria-label="公告业务分类"><button type="button" data-business="all" aria-pressed="true">全部公告</button><button type="button" data-business="GOV_PROCUREMENT" aria-pressed="false">政府采购</button><button type="button" data-business="CONSTRUCTION" aria-pressed="false">建设工程</button></div>
              <form class="search-form" role="search"><label class="sr-only" for="search-input">搜索公告标题或地区</label><span aria-hidden="true">⌕</span><input id="search-input" type="search" placeholder="搜索项目名称、地区" autocomplete="off"><button type="submit" aria-label="搜索公告">→</button></form>
            </div>
            <div class="archive-subtools"><div class="notice-tabs" role="group" aria-label="公告类型"><button type="button" data-type="all" aria-pressed="true">全部类型</button><button type="button" data-type="bidding" aria-pressed="false">招标公告</button><button type="button" data-type="result" aria-pressed="false">结果公告</button><button type="button" data-type="correction" aria-pressed="false">更正公告</button></div><span id="result-count" class="result-count" aria-live="polite"></span></div>
            <div class="announcement-list" id="announcement-list"></div>
            <div class="archive-bottom"><p>资料更新至 <time>${data.snapshot.date.replaceAll('-', '.')}</time></p><button type="button" class="text-link" id="more-announcements">展开更多公告 <span aria-hidden="true">↓</span></button></div>
          </section>
          <section class="services section" id="services" aria-labelledby="services-title"><div class="section-heading"><div><p class="eyebrow section-kicker">02 / EXPERTISE</p><h2 id="services-title">专业服务<span class="heading-period">.</span></h2></div><p class="section-description">贯穿项目全程，让每个环节有据可依。</p></div>
            <div class="services-grid">${services.map((service, i) => `<button class="service-item" type="button" data-service="${i}"><span class="service-number">0${i + 1}</span><svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.3" aria-hidden="true">${service.icon}</svg><span class="service-text"><strong>${service.name}</strong><span>${service.text}</span></span>${arrow}</button>`).join('')}</div>
            <div class="service-callout"><span>有项目需求？<br><strong>和专业的人，聊具体的事。</strong></span><button type="button" data-action="contact" aria-label="咨询项目服务">${arrow}</button></div>
          </section>
        </div>
        <section class="about-section" id="about" aria-labelledby="about-title"><div class="shell about-grid"><div class="about-heading"><p class="eyebrow section-kicker">03 / ABOUT US</p><h2 id="about-title">开放的心态。<br>严谨的专业。</h2><a class="text-link" href="#about" data-action="profile">认识海隆 ${arrow}</a></div><div class="about-content"><p class="about-lead">海隆工程咨询有限公司</p><p class="about-paragraph">${escape(data.profile.text.replace(/\s+/g, ' ').split('“')[0].trim())}</p><div class="archive-stats"><div><strong>${data.snapshot.announcementCount.toLocaleString('en-US')}</strong><span>公开公告归档</span></div><div><strong>${data.snapshot.infoCount}</strong><span>资讯与政策资料</span></div><div><strong>06</strong><span>专业服务方向</span></div></div></div></div></section>
        <section class="insights section shell" id="insights" aria-labelledby="insights-title"><div class="section-heading"><div><p class="eyebrow section-kicker">04 / INSIGHTS</p><h2 id="insights-title">资讯与政策<span class="heading-period">.</span></h2></div><button type="button" class="text-link" id="all-news">查看资料目录 ${arrow}</button></div><div class="insights-grid">${data.news.slice(0, 3).map((article, i) => `<button class="insight-item" type="button" data-news="${article.id}"><div class="insight-meta"><span>${escape(article.category)}</span><time>${article.date.replaceAll('-', '.')}</time></div><div class="insight-visual visual-${i}" aria-hidden="true"><span>${['HL', 'RULES', 'POLICY'][i]}</span><i></i><small>${['交流 / 记录', '规范 / 实践', '资料 / 索引'][i]}</small></div><h3>${escape(article.title)}</h3><div class="insight-bottom"><span>阅读文章</span>${arrow}</div></button>`).join('')}</div></section>
        <section class="contact-section" id="contact"><div class="shell contact-grid"><div><p class="eyebrow">LET’S WORK TOGETHER</p><h2>让下一步，<br>有专业的支撑。</h2></div><div class="contact-details"><span>欢迎与我们联系</span><a class="contact-phone" href="tel:${data.contact.phone}">${data.contact.phone}</a><p>${escape(data.contact.address)}</p><button type="button" class="button contact-button" data-action="contact">查看联系信息 ${arrow}</button></div></div></section>
      </main>
      <footer class="site-footer"><div class="shell footer-top"><a class="brand" href="#home"><img src="assets/logo.png" alt="海隆标志" width="38" height="38"><span><strong>海隆工程咨询</strong><small>HAILONG CONSULTING</small></span></a><p>海纳百川 &nbsp; 才望兼隆</p><a href="#home">返回顶部 <span aria-hidden="true">↑</span></a></div><div class="shell footer-bottom"><span>© ${new Date().getFullYear()} ${escape(data.company.fullName)}</span><span>设计演示 · 资料快照 ${data.snapshot.date} · 建筑视觉为原创插画</span></div></footer>`;
    document.querySelector('#search-input').value = state.query;
    renderAnnouncements();
  }

  function renderAnnouncements() {
    const query = state.query.trim().toLocaleLowerCase();
    const filtered = data.announcements.filter(item => (state.business === 'all' || item.business === state.business) && (state.type === 'all' || item.type === state.type) && (!query || (item.title + item.region).toLocaleLowerCase().includes(query)));
    const limit = state.expanded ? filtered.length : state.style === 'portal' ? 8 : 6;
    document.querySelector('#announcement-list').innerHTML = filtered.length ? filtered.slice(0, limit).map((item, index) => `<button type="button" class="announcement-row" data-announcement="${item.id}" aria-label="阅读：${escape(item.title)}"><span class="row-index">${String(index + 1).padStart(2, '0')}</span><span class="notice-tag tag-${item.type}">${labels[item.type]}</span><span class="row-copy"><strong>${escape(item.title)}</strong><span class="row-region">${businesses[item.business]}<i>·</i>${escape(item.region || '地区未注明')}${item.procurement ? `<i>·</i>${procurement[item.procurement]}` : ''}</span></span><time datetime="${item.date}">${item.date.replaceAll('-', '.')}</time><span class="row-arrow" aria-hidden="true">↗</span></button>`).join('') : '<div class="empty-result"><strong>没有找到匹配公告</strong><p>可以换一个关键词，或清除当前筛选。</p><button type="button" class="text-link" id="clear-search">清除筛选 →</button></div>';
    document.querySelector('#result-count').textContent = `当前样本 ${filtered.length} 条`;
    document.querySelectorAll('[data-business]').forEach(button => button.setAttribute('aria-pressed', String(button.dataset.business === state.business)));
    document.querySelectorAll('[data-type]').forEach(button => button.setAttribute('aria-pressed', String(button.dataset.type === state.type)));
    const more = document.querySelector('#more-announcements');
    more.hidden = filtered.length <= limit && !state.expanded;
    more.innerHTML = state.expanded ? '收起公告 <span aria-hidden="true">↑</span>' : '展开更多公告 <span aria-hidden="true">↓</span>';
  }

  function setStyle(style, save = true) {
    state.style = style;
    document.body.dataset.style = style;
    document.querySelectorAll('[data-style-choice]').forEach(button => button.setAttribute('aria-pressed', String(button.dataset.styleChoice === style)));
    document.title = `${styles[style].name} · 海隆门户设计演示`;
    if (save) {
      try { localStorage.setItem('hailong-design-style', style); } catch {}
      if (location.protocol !== 'file:') { const url = new URL(location.href); url.searchParams.set('style', style); history.replaceState(null, '', url); }
    }
    renderAnnouncements();
  }

  function showDialog(title, body, context = '海隆工程咨询') {
    document.querySelector('#dialog-context').textContent = context;
    content.innerHTML = `<h2 id="dialog-title">${escape(title)}</h2>${body}`;
    // 本项目数据快照已通过初始化 HTML 清理；外链使用独立窗口。
    content.querySelectorAll('a[href^="http"]').forEach(link => { link.target = '_blank'; link.rel = 'noopener noreferrer'; });
    if (!dialog.open) dialog.showModal();
    content.scrollTop = 0;
    document.body.classList.add('dialog-open');
    document.querySelector('#close-dialog').focus();
  }

  function showContact() {
    showDialog('联系我们', `<p class="dialog-intro">具体的项目，值得一次认真沟通。</p><dl class="contact-dl"><div><dt>联系电话</dt><dd><a href="tel:${data.contact.phone}">${data.contact.phone}</a></dd></div><div><dt>电子邮箱</dt><dd><a href="mailto:${data.contact.email}">${escape(data.contact.email)}</a></dd></div><div><dt>公司地址</dt><dd>${escape(data.contact.address)}</dd></div><div><dt>工作时间</dt><dd>${escape(data.contact.hours)}</dd></div></dl><button type="button" class="button primary" id="copy-contact">复制联系信息 <span aria-hidden="true">↗</span></button>`);
  }

  function showDesignNotes() {
    showDialog('四种方向，同一套内容', `<p class="dialog-intro">当前方向：<strong>${styles[state.style].name}</strong>。切换保留筛选状态，便于对比相同信息。</p><div class="design-note-list">${Object.entries(styles).map(([key, style], i) => `<article><div class="design-note-title"><span>0${i + 1}</span><h3>${style.name}</h3><button type="button" class="text-link" data-pick-style="${key}">切换 →</button></div><p>${style.description}</p><dl><div><dt>重点</dt><dd>${style.focus}</dd></div><div><dt>视觉</dt><dd>${style.visual}</dd></div><div><dt>顺序</dt><dd>${style.layout}</dd></div></dl></article>`).join('')}</div><p class="source-note">内容摘自当前项目的初始化资料：24 条公告样本、6 篇资讯资料。归档总量来自 2526 条公告及 43 篇资讯；不以归档数量充当企业业绩，也不展示未补充的荣誉资质。建筑画面是原创插画，不代表公司办公楼或承接项目实景。</p>`, '设计比较');
  }

  function toast(message) { const node = document.querySelector('#toast'); clearTimeout(toastTimer); node.textContent = message; node.classList.add('visible'); toastTimer = setTimeout(() => node.classList.remove('visible'), 2400); }

  document.addEventListener('click', async event => {
    const target = event.target.closest('button, a[data-action]');
    if (!target) return;
    if (target.dataset.styleChoice) { setStyle(target.dataset.styleChoice); return; }
    if (target.dataset.pickStyle) { setStyle(target.dataset.pickStyle); dialog.close(); return; }
    if (target.dataset.business) { state.business = target.dataset.business; state.expanded = false; renderAnnouncements(); return; }
    if (target.dataset.type) { state.type = target.dataset.type; state.expanded = false; renderAnnouncements(); return; }
    if (target.dataset.announcement) {
      const row = data.announcements.find(item => item.id === Number(target.dataset.announcement));
      showDialog(row.title, `<div class="detail-meta"><span>${businesses[row.business]}</span><span>${labels[row.type]}</span><time>${row.date}</time><span>${escape(row.region)}</span></div>${row.budget !== null || row.award !== null ? `<dl class="amount-info">${row.budget !== null ? `<div><dt>预算金额</dt><dd>${row.budget.toLocaleString('zh-CN', {minimumFractionDigits: 2})} <small>万元</small></dd></div>` : ''}${row.award !== null ? `<div><dt>中标 / 成交金额</dt><dd>${row.award.toLocaleString('zh-CN', {minimumFractionDigits: 2})} <small>万元</small></dd></div>` : ''}</dl>` : ''}<div class="document-body">${row.content}</div><p class="source-note">当前初始化资料摘录 · 旧站记录 ${row.id} · 静态演示不提供报名或文件提交。</p>`, '项目公告'); return;
    }
    if (target.dataset.news) { const row = data.news.find(item => item.id === Number(target.dataset.news)); showDialog(row.title, `<div class="detail-meta"><span>${escape(row.category)}</span><time>${row.date}</time></div><div class="document-body">${row.content}</div><p class="source-note">历史资料摘录，日期保留原发布记录。</p>`, '资讯与政策'); return; }
    if (target.dataset.service) { const row = services[Number(target.dataset.service)]; showDialog(row.name, `<p class="dialog-intro">${row.text}</p><ul class="service-detail-list">${row.lines.map(line => `<li>${line}</li>`).join('')}</ul><p>项目范围与具体服务内容，可根据实际需求进一步沟通。</p><button type="button" class="button primary" data-action="contact">联系海隆 ${arrow}</button>`, '专业服务'); return; }
    if (target.dataset.action === 'contact') { showContact(); return; }
    if (target.dataset.action === 'profile') { event.preventDefault(); showDialog('关于海隆', `<div class="document-body">${data.profile.content}</div>`, '企业简介'); return; }
    if (target.id === 'design-notes') { showDesignNotes(); return; }
    if (target.id === 'close-dialog') { dialog.close(); return; }
    if (target.id === 'more-announcements') { state.expanded = !state.expanded; renderAnnouncements(); return; }
    if (target.id === 'clear-search') { state.query = ''; state.business = 'all'; state.type = 'all'; document.querySelector('#search-input').value = ''; renderAnnouncements(); return; }
    if (target.id === 'all-news') { showDialog('资讯与政策资料目录', `<div class="news-directory">${data.news.map(row => `<button type="button" data-news="${row.id}"><span>${escape(row.category)}</span><strong>${escape(row.title)}</strong><time>${row.date}</time>${arrow}</button>`).join('')}</div>`, '历史资料'); return; }
    if (target.id === 'copy-contact') {
      try { await navigator.clipboard.writeText(`${data.company.fullName}\n电话：${data.contact.phone}\n邮箱：${data.contact.email}\n地址：${data.contact.address}`); toast('联系信息已复制'); } catch { toast('当前浏览器不支持复制，请选择文字复制'); } return;
    }
    if (target.classList.contains('menu-toggle')) { const open = target.getAttribute('aria-expanded') !== 'true'; target.setAttribute('aria-expanded', String(open)); document.querySelector('.main-nav').classList.toggle('open', open); target.setAttribute('aria-label', open ? '关闭导航菜单' : '打开导航菜单'); }
  });
  document.addEventListener('click', event => {
    if (event.target.closest('.main-nav a')) { document.querySelector('.main-nav').classList.remove('open'); document.querySelector('.menu-toggle').setAttribute('aria-expanded', 'false'); document.querySelector('.menu-toggle').setAttribute('aria-label', '打开导航菜单'); }
  });
  dialog.addEventListener('close', () => document.body.classList.remove('dialog-open'));
  dialog.addEventListener('click', event => { if (event.target === dialog) { const box = dialog.getBoundingClientRect(); if (event.clientX < box.left || event.clientX > box.right || event.clientY < box.top || event.clientY > box.bottom) dialog.close(); } });
  renderSite();
  document.querySelector('.search-form').addEventListener('submit', event => { event.preventDefault(); state.query = document.querySelector('#search-input').value; state.expanded = false; renderAnnouncements(); });
  document.querySelector('#search-input').addEventListener('input', event => { state.query = event.target.value; state.expanded = false; renderAnnouncements(); });
  setStyle(readSavedStyle(), false);
})();

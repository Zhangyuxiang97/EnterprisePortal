export function buildPageMetadata({ origin, path, company, title, description, noindex = false }) {
  const name = company.fullName || company.name || '海隆工程咨询有限公司'
  return {
    title: title ? `${title} | ${name}` : name,
    description: (description || company.description || name).slice(0, 180),
    canonical: new URL(path, origin).href,
    robots: noindex ? 'noindex, follow' : 'index, follow',
    name
  }
}

export function applyPageMetadata(document, meta, logo) {
  document.title = meta.title
  const setMeta = (attribute, name, content) => {
    let element = document.head.querySelector(`meta[${attribute}="${name}"]`)
    if (!element) { element = document.createElement('meta'); element.setAttribute(attribute, name); document.head.append(element) }
    element.content = content
  }
  setMeta('name', 'description', meta.description)
  setMeta('name', 'robots', meta.robots)
  for (const prefix of ['og', 'twitter']) {
    const attribute = prefix === 'og' ? 'property' : 'name'
    setMeta(attribute, `${prefix}:title`, meta.title)
    setMeta(attribute, `${prefix}:description`, meta.description)
    setMeta(attribute, `${prefix}:url`, meta.canonical)
    setMeta(attribute, `${prefix}:image`, logo)
  }
  setMeta('property', 'og:site_name', meta.name)
  let canonical = document.head.querySelector('link[rel="canonical"]')
  if (!canonical) { canonical = document.createElement('link'); canonical.rel = 'canonical'; document.head.append(canonical) }
  canonical.href = meta.canonical
  let organization = document.getElementById('organization-metadata')
  if (!organization) { organization = document.createElement('script'); organization.id = 'organization-metadata'; organization.type = 'application/ld+json'; document.head.append(organization) }
  organization.textContent = JSON.stringify({ '@context': 'https://schema.org', '@type': 'Organization', name: meta.name, url: new URL('/', meta.canonical).href, logo })
}

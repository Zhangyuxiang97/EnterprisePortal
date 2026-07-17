FROM node:20-alpine AS admin-build
WORKDIR /src/hailong-admin
COPY hailong-admin/package*.json ./
RUN npm ci
COPY hailong-admin/ ./
RUN npm run build

FROM node:20-alpine AS portal-build
WORKDIR /src/hailong-protral
COPY hailong-protral/package*.json ./
RUN npm ci
COPY hailong-protral/ ./
RUN npm run build

FROM nginx:alpine
COPY nginx/nginx.conf /etc/nginx/nginx.conf
COPY nginx/conf.d/default.conf /etc/nginx/conf.d/default.conf
COPY --from=admin-build /src/hailong-admin/dist /usr/share/nginx/html/admin
COPY --from=portal-build /src/hailong-protral/dist /usr/share/nginx/html/portal

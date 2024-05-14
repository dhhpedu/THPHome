import { request } from "@umijs/max";

export const apiBannerList = (params: any) => request(`banner/list`, { params });

export const apiBannerActive = (id: string) => request(`banner/active/${id}`, {
    method: 'POST'
});

export const apiBannerUpdate = (data: any) => request(`banner/update`, {
    method: 'POST',
    data
})

export const apiLogo = (locale: string) => request(`banner/logo`, {
    params: {
        locale
    }
});
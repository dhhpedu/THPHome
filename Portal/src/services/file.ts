import { request } from "@umijs/max";

export const apiFileUpload = (data: any) => request(`file/upload`, {
    method: 'POST',
    data,
    headers: {
        'Content-Type': 'multipart/form-data'
    }
});

export const apiFileList = (params: any) => request(`file/list`, { params  });

export const apiPhotoList = (params: any) => request(`gallery/photo/list`, { params });

export const apiPhotoAdd = (data: any) => request(`gallery/photo/add`, {
    method: 'POST',
    data
})
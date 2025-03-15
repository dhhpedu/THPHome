"use client";

import { apiDepartmentCodeOptions } from "@/services/department";
import { Form, Select } from "antd";
import { useRouter } from "next/navigation";
import { useEffect, useState } from "react";

export const Filter: React.FC = () => {

    const router = useRouter();
    const [options, setOptions] = useState<[]>([]);

    useEffect(() => {
        apiDepartmentCodeOptions().then(response => setOptions(response.data));
    }, []);

    return (
        <div className="flex justify-center md:justify-end gap-2 items-center mb-4 2xl:mb-8">
            <Form layout="vertical">
                <Form.Item name="departmentCode" label="Đơn vị">
                    <Select className="min-w-52" showSearch options={options} placeholder="Chọn đơn vị" onChange={(value) => {
                        if (!value) {
                            return;
                        }
                        router.push(`/?departmentCode=${value}`);
                    }} popupMatchSelectWidth={false} />
                </Form.Item>
            </Form>
        </div>
    )
}
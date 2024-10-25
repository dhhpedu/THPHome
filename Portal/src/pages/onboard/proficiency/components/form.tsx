import { apiAddProficiency } from "@/services/onboard/proficiency";
import { PlusOutlined } from "@ant-design/icons";
import { ModalForm, ProFormDatePicker, ProFormSelect, ProFormText } from "@ant-design/pro-components";
import { Button, Col, message, Row } from "antd";
import { useState } from "react";

type Props = {
    reload: any;
}

const ProFiciencyForm: React.FC<Props> = ({ reload }) => {

    const [open, setOpen] = useState<boolean>(false);

    const onFinish = async (values: any) => {
        await apiAddProficiency(values);
        message.success('Tạo thành công!');
        reload();
        setOpen(false);
    }

    return (
        <>
            <Button type="primary" icon={<PlusOutlined />} onClick={() => setOpen(true)}>Tạo đơn đăng ký</Button>
            <ModalForm open={open} onOpenChange={setOpen} title="Đăng ký chuẩn đầu ra" onFinish={onFinish}>
                <ProFormText name="userName" label="Mã sinh viên" rules={[
                    {
                        required: true
                    }
                ]} />
                <Row gutter={16}>
                    <Col md={12}>
                        <ProFormSelect name="className" label="Lớp" options={[
                            '2-4-6-CN',
                            '3-5-7-CN'
                        ]} rules={[
                            {
                                required: true
                            }
                        ]} />
                    </Col>
                    <Col md={12}>
                        <ProFormSelect name="type" label="Loại" rules={[
                            {
                                required: true
                            }
                        ]} options={[
                            {
                                label: 'Tiếng Anh',
                                value: 0
                            },
                            {
                                label: 'Tiếng Trung',
                                value: 2
                            },
                            {
                                label: 'Tiếng Nhật',
                                value: 3
                            },
                            {
                                label: 'Tin Học',
                                value: 1
                            }
                        ]} />
                    </Col>
                    <Col md={12}>
                        <ProFormSelect name="status" label="Trạng thái" rules={[
                            {
                                required: true
                            }
                        ]} options={[
                            {
                                label: 'Chờ xác nhận',
                                value: 0
                            },
                            {
                                label: 'Đã thanh toán',
                                value: 2
                            }
                        ]} />
                    </Col>
                    <Col md={12}>
                        <ProFormDatePicker name="paymentDate" label="Ngày thanh toán" width="xl" />
                    </Col>
                </Row>
            </ModalForm>
        </>
    )
}

export default ProFiciencyForm;
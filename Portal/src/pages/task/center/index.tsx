import { PageContainer, ProCard, ProDescriptions, ProForm, ProFormTextArea, ProList } from "@ant-design/pro-components"
import { history, useParams, useRequest } from "@umijs/max"
import { apiTaskItemDetail, apiTaskItemHistoryList } from "../services/task-item"
import { Button, Divider} from "antd";
import { CalendarOutlined, LeftOutlined, PlusOutlined, UserOutlined } from "@ant-design/icons";
import 'ckeditor5/ckeditor5.css';
import { TaskPriorityList, TaskStatusList } from "../constants";
import AssignModal from "./components/assign";
import dayjs from "dayjs";
import StatusChange from "./components/status";

const Index: React.FC = () => {

    const { id } = useParams<{ id: string }>();
    const { data, refresh } = useRequest(() => apiTaskItemDetail(id));

    return (
        <PageContainer title={data?.title} extra={<Button icon={<LeftOutlined />} onClick={() => history.back()}>Quay lại</Button>}>
            <div className="md:flex gap-4">
                <div className="md:w-2/3">
                    <ProCard title="Thông tin nhiệm vụ" className="mb-4" headerBordered>
                        <div className="ck ck-editor">
                            <div dangerouslySetInnerHTML={{ __html: data?.content }} className="ck ck-content"></div>
                        </div>
                        <Divider orientation="left">Tệp đính kèm</Divider>
                        <div className="flex gap-2">
                            <div className="h-16 w-16 bg-gray-100 border border-dashed hover:border-blue-500 rounded flex items-center justify-center">
                                <PlusOutlined />
                            </div>
                        </div>
                    </ProCard>
                    <ProCard title="Bình luận" className="mb-4" headerBordered>
                        <ProForm disabled>
                            <ProFormTextArea label="Bình luận" name="comment" placeholder="Nhập bình luận" rows={4} />
                        </ProForm>
                    </ProCard>
                </div>
                <div className="md:w-1/3">
                    <ProCard title="Cài đặt" className="mb-4"
                        headerBordered>
                        <ProDescriptions column={1} bordered size="small">
                            <ProDescriptions.Item label="Người thực hiện">
                                <UserOutlined className="mr-1" />
                                {data?.assignedTo}
                                <AssignModal refresh={refresh} />
                            </ProDescriptions.Item>
                            <ProDescriptions.Item label="Ngày bắt đầu" valueType="date">{data?.startDate}</ProDescriptions.Item>
                            <ProDescriptions.Item label="Ngày hết hạn" valueType="date">{data?.dueDate}</ProDescriptions.Item>
                            <ProDescriptions.Item label="Trạng thái" valueType="select" fieldProps={{
                                options: TaskStatusList
                            }}>{data?.status}<StatusChange refresh={refresh} /></ProDescriptions.Item>
                            <ProDescriptions.Item label="Độ ưu tiên"
                                valueType="select"
                                fieldProps={{
                                    options: TaskPriorityList
                                }}
                            >{data?.priority}</ProDescriptions.Item>
                            <ProDescriptions.Item label="Người tạo">{data?.createdBy}</ProDescriptions.Item>
                            <ProDescriptions.Item label="Ngày tạo" valueType="date">{data?.createdDate}</ProDescriptions.Item>
                            <ProDescriptions.Item label="Người sửa">{data?.modifiedBy}</ProDescriptions.Item>
                            <ProDescriptions.Item label="Ngày sửa" valueType="date">{data?.modifiedDate}</ProDescriptions.Item>
                        </ProDescriptions>
                    </ProCard>
                    <ProCard title="Lịch sử thay đổi" className="mb-4" headerBordered>
                        <ProList request={(params) => apiTaskItemHistoryList({
                            ...params,
                            taskItemId: id
                        })}
                        metas={{
                            description: {
                                dataIndex: 'action',
                                render: (_, record) => (
                                    <div>
                                        <div className="text-slate-900 font-medium">{record.action}</div>
                                        <div className="text-gray-500"><CalendarOutlined /> {dayjs(record.createdDate).format('DD/MM/YYYY')}</div>
                                    </div>
                                )
                            }
                        }}
                        />
                    </ProCard>
                </div>
            </div>
        </PageContainer>
    )
}

export default Index
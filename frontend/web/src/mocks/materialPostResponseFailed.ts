import { IMaterialPostRes } from 'src/app/model/http/materialReques';

export const MaterialPostResFailedMock: IMaterialPostRes = {
    code: 200,
    comments: 118,
    exceptionMessage: null,
    success: true,
    userError: null,
    response: {
        failed: [
            {
                productionOrderId: 1235,
                reason: 'Algo falló',
            }
        ],
    }
};

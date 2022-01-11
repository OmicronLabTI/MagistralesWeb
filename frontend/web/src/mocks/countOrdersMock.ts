import { IQfbWithNumberRes } from 'src/app/model/http/users';

export const CountOrdersMock: IQfbWithNumberRes = {
    code: 200,
    userError: null,
    exceptionMessage: null,
    success: true,
    response: [
        {
            userId: 'fedea474-428a-4226-838f-7bb263deb939',
            userName: 'Janette Zavala',
            countTotalFabOrders: 2,
            countTotalOrders: 2,
            countTotalPieces: 4,
            asignable: 1,
            clasification: 'MG'
        },
        {
            userId: '6bc7f8a8-8617-43ac-a804-79cf9667b8ae',
            userName: 'karen lopez',
            countTotalFabOrders: 96,
            countTotalOrders: 75,
            countTotalPieces: 464,
            asignable: 1,
            clasification: 'MN'
        },
        {
            userId: 'eec20193-5f2a-4911-bc08-66d5bd8c9962',
            userName: 'Sergio Flores',
            countTotalFabOrders: 42,
            countTotalOrders: 32,
            countTotalPieces: 124,
            asignable: 1,
            clasification: 'MN'
        },
        {
            userId: 'e320fcc8-9904-4ed1-98aa-ffdb44567eff',
            userName: 'Tania Domínguez QFB',
            countTotalFabOrders: 34,
            countTotalOrders: 23,
            countTotalPieces: 220,
            asignable: 1,
            clasification: 'BE'
        }
    ],
    comments: null
};

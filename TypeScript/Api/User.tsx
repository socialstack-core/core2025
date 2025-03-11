// File generated from the C# definition
declare global {
    interface User {

        role: int

    }
}

interface UserApi {
    get: (userId: int) => Promise<User>;
}

const userApi: UserApi = {
    get: (userId: int) => {
        return new Promise((s, r) => {
            s({});
        });
    }
};

export default userApi;
declare global {

    interface Role {
        capabilities?: string[]
    }

}

interface RoleApi {
    get: (roleId: int) => Promise<Role>;
}

const roleApi : RoleApi = {
    get: (roleId: int) => {
        return new Promise((s, r) => {
            s({});
        });
    }
};

export default roleApi;
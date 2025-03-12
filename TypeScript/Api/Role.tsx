import { AutoApi, VersionedContent } from 'TypeScript/Api/ApiEndpoints'

// Module
export type Role = VersionedContent<int> & {
    capabilities?: string[]
}

class RoleApi extends AutoApi<Role>{
    public constructor() {
        super('v1/role')
    }

}

const rApi = new RoleApi();
export default rApi;


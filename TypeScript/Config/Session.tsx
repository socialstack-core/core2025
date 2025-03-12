import { User } from 'TypeScript/Api/User';
import { Role } from 'Api/Role';

// File generated from the C# definition

declare global {

    interface Session {

        /**
         * Set when the UI is currently waiting for the sessions user info to load.
         */
        loadingUser?: Promise<Session>

        /**
         * The current user account.
         */
        user?: User
        
        /**
         * Optionally provided as the 'real' user when the current user is impersonating someone else.
         * Use sparingly as overuse would of course make impersonation relatively meaningless.
         */
        realUser?: User

        /**
         * The current user's role.
         */
        role?: Role

    }
}

export { };
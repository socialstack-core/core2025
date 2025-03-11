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
         * The current user's role.
         */
        role?: Role

    }
}

export { };
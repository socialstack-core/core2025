import { ApiContent } from 'UI/Functions/WebRequest';
import { Role } from 'Api/Role';
import { SiteDomain } from 'Api/SiteDomain';
import { Locale } from 'Api/Locale';
import { User } from 'Api/User';
// File generated from the C# definition

declare global {

    interface Session {

        /**
         * Set when the UI is currently waiting for the sessions user info to load.
         */
        loadingUser?: Promise<Session>

        /**
         * Optionally provided as the 'real' user when the current user is impersonating someone else.
         * Use sparingly as overuse would of course make impersonation relatively meaningless.
         */
        realUser?: User

		/**
         * The current session Role
         */
        role?: Role

		/**
         * The current session SiteDomain
         */
        sitedomain?: SiteDomain

		/**
         * The current session Locale
         */
        locale?: Locale

		/**
         * The current session User
         */
        user?: User

    }
    interface SessionResponse {

		/**
         * The pre-expanded Role
         */
        role?: ApiContent<Role>

		/**
         * The pre-expanded SiteDomain
         */
        sitedomain?: ApiContent<SiteDomain>

		/**
         * The pre-expanded Locale
         */
        locale?: ApiContent<Locale>

		/**
         * The pre-expanded User
         */
        user?: ApiContent<User>

	}
}

export { };
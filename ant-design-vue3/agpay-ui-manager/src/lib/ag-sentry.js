/*
 * 错误上报sentry
 *
 */

export const smartSentry = {
    /**
     * sentry 主动上报
     */
    captureError: (error) => {
        if (error.config && error.data && error && error.headers && error.request && error.status) {
            return;
        }
        // Sentry.captureException(error);
        console.error(error);
    },
};

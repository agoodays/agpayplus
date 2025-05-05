/*
 * loading
 *
 */
import { defineStore } from 'pinia';

export const useSpinStore = defineStore('spin', {
  state: () => ({
    loading: false,
  }),

  actions: {
    hide() {
      this.loading = false;
    },
    show() {
      this.loading = true;
    },
  },
});

import { configureStore } from "@reduxjs/toolkit"
import { salesIntelApi } from "./api/salesIntelApi"

export const store = configureStore({
  reducer: {
    [salesIntelApi.reducerPath]: salesIntelApi.reducer,
  },
  middleware: (getDefaultMiddleware) => getDefaultMiddleware().concat(salesIntelApi.middleware),
})

export type RootState = ReturnType<typeof store.getState>
export type AppDispatch = typeof store.dispatch

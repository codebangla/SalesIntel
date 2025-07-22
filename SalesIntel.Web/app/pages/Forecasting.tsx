"use client"

import { useState } from "react"
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card"
import { Tabs, TabsContent, TabsList, TabsTrigger } from "@/components/ui/tabs"
import { Button } from "@/components/ui/button"
import { Label } from "@/components/ui/label"
import { Slider } from "@/components/ui/slider"
import { Input } from "@/components/ui/input"
import { RefreshCw } from "lucide-react"
import { useGenerateForecastMutation, useGetInventoryAlertsQuery } from "@/store/api/salesIntelApi"
import { ForecastChart } from "@/components/ForecastChart"
import { InventoryAlerts } from "@/components/InventoryAlerts"
import type { ForecastDto } from "@/store/api/salesIntelApi"

export function Forecasting() {
  const [forecastParams, setForecastParams] = useState({
    shortTermPeriod: 7,
    trendPeriod: 30,
    seasonalPeriod: 90,
    resourceFactor: 1.2,
  })

  const [forecastData, setForecastData] = useState<ForecastDto[]>([])
  const [generateForecast, { isLoading: isGenerating }] = useGenerateForecastMutation()
  const { data: alerts = [] } = useGetInventoryAlertsQuery()

  const updateParam = (key: string, value: number) => {
    setForecastParams({ ...forecastParams, [key]: value })
  }

  const handleGenerateForecast = async () => {
    try {
      const result = await generateForecast(forecastParams).unwrap()
      setForecastData(result)
    } catch (error) {
      console.error("Failed to generate forecast:", error)
    }
  }

  return (
    <div className="container mx-auto py-6 px-4">
      <div className="flex items-center justify-between mb-6">
        <h1 className="text-3xl font-bold">Forecasting</h1>
      </div>

      <div className="space-y-6">
        <Card>
          <CardHeader>
            <CardTitle>Forecasting Parameters</CardTitle>
            <CardDescription>Adjust the parameters to fine-tune your forecasting models</CardDescription>
          </CardHeader>
          <CardContent>
            <div className="grid gap-6 md:grid-cols-2 lg:grid-cols-4">
              <div className="space-y-2">
                <Label htmlFor="short-term">Short-term Period (days)</Label>
                <div className="space-y-2">
                  <Slider
                    id="short-term"
                    min={3}
                    max={30}
                    step={1}
                    value={[forecastParams.shortTermPeriod]}
                    onValueChange={(value) => updateParam("shortTermPeriod", value[0])}
                  />
                  <Input
                    type="number"
                    value={forecastParams.shortTermPeriod}
                    onChange={(e) => updateParam("shortTermPeriod", Number.parseInt(e.target.value) || 7)}
                    className="h-8"
                  />
                </div>
              </div>

              <div className="space-y-2">
                <Label htmlFor="trend">Trend Period (days)</Label>
                <div className="space-y-2">
                  <Slider
                    id="trend"
                    min={7}
                    max={90}
                    step={1}
                    value={[forecastParams.trendPeriod]}
                    onValueChange={(value) => updateParam("trendPeriod", value[0])}
                  />
                  <Input
                    type="number"
                    value={forecastParams.trendPeriod}
                    onChange={(e) => updateParam("trendPeriod", Number.parseInt(e.target.value) || 30)}
                    className="h-8"
                  />
                </div>
              </div>

              <div className="space-y-2">
                <Label htmlFor="seasonal">Seasonal Period (days)</Label>
                <div className="space-y-2">
                  <Slider
                    id="seasonal"
                    min={30}
                    max={365}
                    step={1}
                    value={[forecastParams.seasonalPeriod]}
                    onValueChange={(value) => updateParam("seasonalPeriod", value[0])}
                  />
                  <Input
                    type="number"
                    value={forecastParams.seasonalPeriod}
                    onChange={(e) => updateParam("seasonalPeriod", Number.parseInt(e.target.value) || 90)}
                    className="h-8"
                  />
                </div>
              </div>

              <div className="space-y-2">
                <Label htmlFor="resource">Resource Factor</Label>
                <div className="space-y-2">
                  <Slider
                    id="resource"
                    min={0.5}
                    max={3.0}
                    step={0.1}
                    value={[forecastParams.resourceFactor]}
                    onValueChange={(value) => updateParam("resourceFactor", value[0])}
                  />
                  <Input
                    type="number"
                    step="0.1"
                    value={forecastParams.resourceFactor}
                    onChange={(e) => updateParam("resourceFactor", Number.parseFloat(e.target.value) || 1.2)}
                    className="h-8"
                  />
                </div>
              </div>
            </div>

            <div className="flex justify-end mt-6">
              <Button onClick={handleGenerateForecast} disabled={isGenerating}>
                <RefreshCw className={`mr-2 h-4 w-4 ${isGenerating ? "animate-spin" : ""}`} />
                {isGenerating ? "Generating..." : "Generate Forecast"}
              </Button>
            </div>
          </CardContent>
        </Card>

        <Tabs defaultValue="forecast" className="space-y-4">
          <TabsList>
            <TabsTrigger value="forecast">Forecast Analysis</TabsTrigger>
            <TabsTrigger value="alerts">Inventory Alerts</TabsTrigger>
          </TabsList>

          <TabsContent value="forecast" className="space-y-4">
            <Card>
              <CardHeader>
                <CardTitle>Sales Forecast</CardTitle>
                <CardDescription>
                  Predictive analysis using moving average, linear regression, and seasonal patterns
                </CardDescription>
              </CardHeader>
              <CardContent>
                <ForecastChart data={forecastData} />
              </CardContent>
            </Card>
          </TabsContent>

          <TabsContent value="alerts" className="space-y-4">
            <InventoryAlerts alerts={alerts} />
          </TabsContent>
        </Tabs>
      </div>
    </div>
  )
}

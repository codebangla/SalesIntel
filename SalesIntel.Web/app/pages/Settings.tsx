"use client"

import { Button } from "@/components/ui/button"
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card"
import { useReloadSeedDataMutation } from "@/store/api/salesIntelApi"
import { useToast } from "@/hooks/use-toast"

export function Settings() {
  const [reloadSeedData, { isLoading }] = useReloadSeedDataMutation()
  const { toast } = useToast()

  const handleReloadSeedData = async () => {
    try {
      await reloadSeedData().unwrap()
      toast({
        title: "Success",
        description: "Seed data reloaded successfully!",
      })
    } catch (error) {
      toast({
        title: "Error",
        description: "Failed to reload seed data",
        variant: "destructive",
      })
    }
  }

  return (
    <div className="container mx-auto py-6 px-4">
      <div className="flex items-center justify-between mb-6">
        <h1 className="text-3xl font-bold">Settings</h1>
      </div>

      <div className="space-y-6">
        <Card>
          <CardHeader>
            <CardTitle>Database Management</CardTitle>
            <CardDescription>Manage your application data</CardDescription>
          </CardHeader>
          <CardContent>
            <Button onClick={handleReloadSeedData} disabled={isLoading} variant="outline">
              {isLoading ? "Reloading..." : "Reload Seed Data"}
            </Button>
          </CardContent>
        </Card>
      </div>
    </div>
  )
}

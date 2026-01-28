$ErrorActionPreference = 'Stop'

dotnet test
if ($LASTEXITCODE -eq 0) {
  Write-Host "TESTS PASSED" -ForegroundColor Green
  exit 0
}

Write-Host "TESTS FAILED" -ForegroundColor Red
exit $LASTEXITCODE

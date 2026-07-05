# Kubernetes Configuration

Kubernetes manifests and Helm charts for Naar-Noor multi-tier application.

## Directory Structure

```
kubernetes/
├── base/                    # Base Kubernetes manifests (kustomization)
│   ├── namespace.yaml
│   ├── secrets.yaml
│   ├── configmaps.yaml
│   ├── services.yaml
│   ├── backend-deployment.yaml
│   ├── frontend-deployment.yaml
│   ├── database-statefulset.yaml
│   ├── ingress.yaml
│   ├── hpa.yaml
│   ├── service-accounts.yaml
│   ├── rbac.yaml
│   └── kustomization.yaml
├── overlays/
│   ├── dev/                 # Development configuration
│   │   ├── kustomization.yaml
│   │   └── patches/
│   ├── staging/             # Staging configuration
│   │   ├── kustomization.yaml
│   │   └── patches/
│   └── production/          # Production configuration
│       ├── kustomization.yaml
│       └── patches/
├── helm/                    # Helm charts for packaging
│   ├── naar-noor/
│   │   ├── Chart.yaml
│   │   ├── values.yaml
│   │   ├── values-dev.yaml
│   │   ├── values-staging.yaml
│   │   ├── values-production.yaml
│   │   └── templates/
│   └── README.md
└── README.md
```

## Quick Start

### Deploy to Local Kubernetes (Development)

```bash
# Using kubectl directly
kubectl apply -f base/

# Or using Kustomize
kubectl apply -k overlays/dev/

# View deployment
kubectl get all -n naar-noor
```

### Deploy to Staging

```bash
kubectl apply -k overlays/staging/
```

### Deploy to Production

```bash
kubectl apply -k overlays/production/
```

## Using Helm (Recommended for Production)

### Install

```bash
cd helm/naar-noor

# Add any required Helm repositories (if using dependencies)
# helm repo add <repo-name> <repo-url>

# Install chart
helm install naar-noor . -f values-production.yaml -n naar-noor --create-namespace
```

### Upgrade

```bash
helm upgrade naar-noor . -f values-production.yaml -n naar-noor
```

### Uninstall

```bash
helm uninstall naar-noor -n naar-noor
```

## Configuration

### Environment-Specific Patches (Kustomize)

#### Development (`overlays/dev/kustomization.yaml`)
- 1 replica per deployment
- Resource limits: 200m CPU, 256Mi memory
- Development-grade security

#### Staging (`overlays/staging/kustomization.yaml`)
- 2 replicas per deployment
- Resource limits: 500m CPU, 512Mi memory
- Staging-grade monitoring

#### Production (`overlays/production/kustomization.yaml`)
- 3+ replicas per deployment
- Resource limits: 1000m CPU, 1Gi memory
- Full monitoring and logging
- Auto-scaling enabled
- Pod disruption budgets

### Helm Values

```yaml
# values-production.yaml
replicaCount: 3
environment: production

backend:
  image: ghcr.io/owner/naar-noor:backend-latest
  replicas: 3
  resources:
    requests:
      cpu: 200m
      memory: 256Mi
    limits:
      cpu: 1000m
      memory: 1Gi

frontend:
  image: ghcr.io/owner/naar-noor:frontend-latest
  replicas: 2
  resources:
    requests:
      cpu: 100m
      memory: 128Mi
    limits:
      cpu: 500m
      memory: 512Mi

database:
  replicas: 1
  storage: 100Gi
  backup: enabled
```

## Manifests Explained

### Namespace
- Logical isolation for all Naar-Noor resources
- Default: `naar-noor`

### Services
- **backend-service**: ClusterIP (internal)
- **frontend-service**: LoadBalancer or ClusterIP + Ingress
- **database-service**: ClusterIP (internal, no external access)

### Deployments
- **backend**: ASP.NET Core API (3 replicas)
- **frontend**: Angular web app (2 replicas)

### StatefulSet
- **database**: PostgreSQL with persistent storage
- One pod with 100Gi storage
- Headless service for stable DNS

### Ingress
- HTTP/HTTPS routing
- TLS certificate (Let's Encrypt or custom)
- Paths:
  - `/` → frontend-service
  - `/api` → backend-service

### HPA (Horizontal Pod Autoscaling)
- Backend: scale 2-10 pods (CPU 70% threshold)
- Frontend: scale 2-5 pods (CPU 80% threshold)

### Secrets
- Database credentials
- API keys
- TLS certificates
- Encoded in base64 (not encrypted by default)

### ConfigMaps
- Application configuration
- Non-sensitive environment variables
- Feature flags

### RBAC (Role-Based Access Control)
- Service accounts for each component
- Minimal permissions (principle of least privilege)
- Roles for reading configmaps/secrets

## Health Checks

### Liveness Probe
- Restarts pod if probe fails
- Endpoint: `GET /health`
- Initial delay: 15 seconds
- Period: 10 seconds

### Readiness Probe
- Removes pod from load balancer if probe fails
- Endpoint: `GET /health`
- Initial delay: 10 seconds
- Period: 5 seconds

## Resource Management

### Requests (guaranteed resources)
- Backend: 200m CPU, 256Mi memory
- Frontend: 100m CPU, 128Mi memory

### Limits (maximum resources)
- Backend: 1000m CPU, 1Gi memory
- Frontend: 500m CPU, 512Mi memory

## Security

### Pod Security
- Non-root user (UID 1000)
- Read-only root filesystem
- No privilege escalation
- Dropped Linux capabilities

### Network
- Service accounts with limited permissions
- Secrets mounted as volumes (not env vars)
- Pod anti-affinity for high availability

## Monitoring

### Prometheus Annotations
```yaml
prometheus.io/scrape: "true"
prometheus.io/port: "8080"
prometheus.io/path: "/metrics"
```

### Custom Metrics
- Application metrics exposed on `/metrics`
- Prometheus scrapes every 30 seconds

## Persistence

### Database Storage
- StatefulSet with 100Gi PersistentVolumeClaim
- Storage class: `standard` (or custom)
- Retained on deployment updates

### Backups
- Daily snapshots (managed by cloud provider)
- Point-in-time restore capability

## Networking

### Internal Communication
- Backend ↔ Database: via service DNS
- Frontend ↔ Backend: via Ingress or API Gateway

### External Access
- Frontend: HTTP/HTTPS via Ingress
- Backend API: HTTP via Ingress (/api prefix)
- Database: Not exposed externally

## Deployment Strategies

### Rolling Update (default)
- Gradually replace old pods with new ones
- Zero downtime
- Configuration:
  - Max surge: 1 pod
  - Max unavailable: 0 pods

### Blue-Green (optional)
- Run two parallel versions
- Switch traffic instantly
- No downtime, easy rollback

### Canary (optional)
- Route small percentage of traffic to new version
- Gradual increase if metrics are healthy
- Automatic rollback on errors

## Common Operations

### View Logs

```bash
# Pod logs
kubectl logs <pod-name> -n naar-noor

# Follow logs in real-time
kubectl logs -f <pod-name> -n naar-noor

# Multiple pods
kubectl logs -f -l app=naar-noor,component=backend -n naar-noor
```

### Execute Commands

```bash
# Interactive shell
kubectl exec -it <pod-name> -n naar-noor -- /bin/bash

# Run command
kubectl exec <pod-name> -n naar-noor -- curl http://localhost/health
```

### Port Forwarding

```bash
# Forward local port to pod
kubectl port-forward <pod-name> 8080:80 -n naar-noor

# Access at http://localhost:8080
```

### Scaling

```bash
# Scale manually
kubectl scale deployment naar-noor-backend --replicas=5 -n naar-noor

# Check rollout status
kubectl rollout status deployment/naar-noor-backend -n naar-noor
```

### Rollback

```bash
# View rollout history
kubectl rollout history deployment/naar-noor-backend -n naar-noor

# Rollback to previous version
kubectl rollout undo deployment/naar-noor-backend -n naar-noor

# Rollback to specific revision
kubectl rollout undo deployment/naar-noor-backend --to-revision=2 -n naar-noor
```

## Troubleshooting

### Pod Not Starting
```bash
kubectl describe pod <pod-name> -n naar-noor
kubectl logs <pod-name> -n naar-noor
```

### CrashLoopBackOff
```bash
# Check pod events
kubectl get events -n naar-noor --sort-by='.lastTimestamp'

# Check logs before crash
kubectl logs --previous <pod-name> -n naar-noor
```

### Pending Pods
```bash
# Check node resources
kubectl top nodes

# Check PVC status
kubectl get pvc -n naar-noor
kubectl describe pvc <pvc-name> -n naar-noor
```

## Checklist for Production

- [ ] Images pushed to registry (GHCR/ECR/ACR)
- [ ] Secrets configured and stored securely
- [ ] Ingress configured with TLS
- [ ] HPA properly configured for autoscaling
- [ ] Resource limits set appropriately
- [ ] Health checks working correctly
- [ ] Monitoring and logging configured
- [ ] Backup and disaster recovery plan in place
- [ ] Network policies enforced
- [ ] RBAC properly configured
- [ ] Pod disruption budgets defined
- [ ] Load testing completed

---

**Status**: Production Ready  
**Last Updated**: July 5, 2026

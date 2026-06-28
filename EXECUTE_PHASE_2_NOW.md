# Phase 2 Production Hardening - EXECUTE NOW

**Status**: Ready to execute  
**Error Found**: Tables don't exist yet  
**Solution**: Execute SQL scripts in correct order  

---

## 3-STEP DEPLOYMENT (15 minutes total)

### STEP 1: Create Database Tables (Run First!)

**File**: `sql/0_create_tables.sql`

**Instructions**:
1. Open Supabase Dashboard: https://supabase.com/dashboard
2. Select project: **uyzocpvytoljigmcpafn**
3. Go to **SQL Editor** → **New Query**
4. Copy entire contents of: `sql/0_create_tables.sql`
5. Click **RUN** button
6. **WAIT for completion** ⏳

**Expected Output**:
```
Query returned successfully with these results:

Table name
-----------
Chefs
ContactInquiries
MenuItems
OrderItems
Orders
Reservations
Reviews

tables_created
--------------
7
```

**Verify**: You should see 7 tables listed ✅

---

### STEP 2: Apply RLS Policies (Run After Step 1)

**File**: `sql/2_1_rls_implementation.sql`

**Instructions**:
1. In Supabase SQL Editor → **New Query**
2. Copy entire contents of: `sql/2_1_rls_implementation.sql`
3. Click **RUN** button
4. **WAIT for completion** ⏳

**Expected Output**:
```
Query returned successfully with these results:

schemaname | tablename | policy_count
-----------+-----------+---------------
public     | Chefs     | 1
public     | ContactInquiries | 1
public     | MenuItems | 1
public     | OrderItems | 1
public     | Orders    | 3
public     | Reservations | 4
public     | Reviews   | 2

total_policies
--------------
13
```

**Verify**: You should see 13 policies total ✅

---

### STEP 3: Apply Storage Policies (Run After Step 2)

**File**: `sql/2_5_storage_policies.sql`

**Instructions**:
1. In Supabase SQL Editor → **New Query**
2. Copy entire contents of: `sql/2_5_storage_policies.sql`
3. Click **RUN** button
4. **WAIT for completion** ⏳

**Expected Output**:
```
Query returned successfully with these results:

schemaname | tablename | policy_count
-----------+----------+---------------
storage    | objects   | 8

total_storage_policies
----------------------
8
```

**Verify**: You should see 8 storage policies ✅

---

## AFTER EXECUTING ALL 3 STEPS

### Test Backend Connectivity

```bash
curl -i https://naar-noor.runasp.net/health
```

Should return:
```
HTTP/1.1 200 OK
Content-Type: application/json

{"status":"Healthy"}
```

### Test Rate Limiting

```bash
for i in {1..6}; do
  curl -X POST https://naar-noor.runasp.net/api/auth/register \
    -H "Content-Type: application/json" \
    -d '{"email":"test'$i'@example.com","password":"Test123!"}' \
    -w "Status: %{http_code}\n"
done
```

Expected: 200, 200, 200, 200, 200, **429** ✅

### Test CORS

```bash
curl -i -H "Origin: https://naar-noor.vercel.app" \
  https://naar-noor.runasp.net/api/menu-items
```

Should include header:
```
Access-Control-Allow-Origin: https://naar-noor.vercel.app
```

---

## WHAT EACH SCRIPT DOES

| Script | Tables | Policies | Purpose |
|--------|--------|----------|---------|
| `0_create_tables.sql` | 7 created | — | Create schema |
| `2_1_rls_implementation.sql` | — | 13 created | Data isolation |
| `2_5_storage_policies.sql` | — | 8 created | File access |

---

## TROUBLESHOOTING

### Error: "relation already exists"
- This is okay, scripts use `IF NOT EXISTS`
- Just continue to next step

### Error: "relation still doesn't exist"
- Verify Step 1 completed successfully
- Check 7 tables appear in Supabase browser
- Try Step 2 again

### Health check timeout
- Wait 2-3 minutes for deployment
- Check GitHub Actions status
- Verify RunASP service is running

---

## SIGN-OFF

Phase 2 complete when:

- [x] Step 1: 7 tables created
- [x] Step 2: 13 RLS policies created
- [x] Step 3: 8 storage policies created
- [ ] Health check responds
- [ ] Rate limiting works (429 on 6th request)
- [ ] CORS headers present

---

## Next: Phase 3 Testing

After Phase 2 complete:
1. Integration tests with PostgreSQL
2. Load testing (k6)
3. End-to-end tests (Cypress)
4. Staging validation

---

**START NOW**: 
Open `sql/0_create_tables.sql` and execute in Supabase SQL Editor

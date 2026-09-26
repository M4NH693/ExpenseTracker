import psycopg2

try:
    conn = psycopg2.connect("host=localhost dbname=quan_ly_chi_tieu user=postgres password=manhdz123 client_encoding=utf8")
    conn.autocommit = True
    cursor = conn.cursor()
    
    with open("database.sql", "r", encoding="utf-8") as f:
        sql = f.read()
        
    cursor.execute(sql)
    print("Database reset successfully!")
    
except Exception as e:
    print("Error:", e)
finally:
    if 'conn' in locals():
        conn.close()

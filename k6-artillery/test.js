import http from 'k6/http';
import { check, sleep } from 'k6';
import { SharedArray } from 'k6/data';

const tokens = new SharedArray('tokens', function () {
    return [
        { "value": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxIiwiZW1haWwiOiJ0ZXN0QGVtYWlsLmNvbSIsImp0aSI6ImNjZjlmYzY5LWRhMzktNGNhYi1iYmY0LWNjMTg3OTk5MWVlMSIsImV4cCI6MTc1MjA3MDAyMX0.OTtltK6Hm7b_oXEhQ8QGIb6XOz0jw_u3-IQkXJ1u7II" }
    ];
});

export const options = {
    vus: 10,
    duration: '30s',

    thresholds: {
        'http_req_duration': ['p(95)<900'],
        'http_req_failed': ['rate<0.01'], 
        'checks': ['rate>0.99'],        
    },
};

export default function () {
    const token = tokens[0].value;

    const headers = {
        'Content-Type': 'application/json',
        'Authorization': `Bearer ${token}`,
    };
    const url = 'http://localhost:5010/v1/users';

    const res = http.get(url, { headers: headers });

    check(res, {
        'status is 200': (r) => r.status === 200,
        'response body contains UsersWithRoles': (r) => {
            try {
                const body = JSON.parse(r.body);
                return body && Array.isArray(body.usersWithRoles);
            } catch (e) {
                return false;
            }
        },
        'response body is not empty': (r) => {
            try {
                const body = JSON.parse(r.body);
                return body.usersWithRoles.length > 0;
            } catch (e) {
                return false;
            }
        }
    });
    sleep(1);
}
import http from 'k6/http';
import { check, sleep } from 'k6';
import { SharedArray } from 'k6/data';

// Dane testowe - np. token JWT
// UWAGA: W środowisku produkcyjnym, tokeny powinny być przechowywane bezpiecznie
// i rotowane. Tutaj dla celów testowych jest zakodowany na stałe.
const tokens = new SharedArray('tokens', function () {
    // Tutaj możesz dodać więcej tokenów, jeśli chcesz testować z różnymi użytkownikami
    return [
        { "value": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxIiwiZW1haWwiOiJ0ZXN0QGVtYWlsLmNvbSIsImp0aSI6ImNjZjlmYzY5LWRhMzktNGNhYi1iYmY0LWNjMTg3OTk5MWVlMSIsImV4cCI6MTc1MjA3MDAyMX0.OTtltK6Hm7b_oXEhQ8QGIb6XOz0jw_u3-IQkXJ1u7II" }
    ];
});

export const options = {
    // Prosty test obciążeniowy
    vus: 10,  // 10 wirtualnych użytkowników
    duration: '30s', // Test trwa 30 sekund

    // Progi sukcesu/porażki
    thresholds: {
        'http_req_duration': ['p(95)<900'], // 95% żądań powinno trwać krócej niż 500ms
        'http_req_failed': ['rate<0.01'],  // Mniej niż 1% błędów żądań HTTP
        'checks': ['rate>0.99'],           // Więcej niż 99% wykonanych sprawdzeń powinno zakończyć się sukcesem
    },
};

export default function () {
    // Użyj pierwszego dostępnego tokenu z SharedArray
    const token = tokens[0].value;

    // Nagłówki żądania, w tym nagłówek autoryzacji
    const headers = {
        'Content-Type': 'application/json',
        'Authorization': `Bearer ${token}`,
    };

    // Adres URL endpointu GetUsersWithRoles
    const url = 'http://localhost:5010/v1/users'; // Zmień na swój aktualny adres URL API

    // Wykonaj żądanie GET
    const res = http.get(url, { headers: headers });

    // Sprawdzenia (assertions) odpowiedzi
    check(res, {
        'status is 200': (r) => r.status === 200, // Sprawdź, czy status HTTP to 200 OK
        'response body contains UsersWithRoles': (r) => {
            try {
                const body = JSON.parse(r.body);
                return body && Array.isArray(body.usersWithRoles); // Sprawdź, czy istnieje tablica usersWithRoles
            } catch (e) {
                return false;
            }
        },
        'response body is not empty': (r) => {
            try {
                const body = JSON.parse(r.body);
                // Sprawdź, czy tablica usersWithRoles nie jest pusta, jeśli oczekujesz danych
                return body.usersWithRoles.length > 0;
            } catch (e) {
                return false;
            }
        }
    });

    // Pacing - poczekaj 1 sekundę przed kolejnym żądaniem
    sleep(1);
}